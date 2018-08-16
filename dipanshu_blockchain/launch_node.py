# Copyright 2015 gRPC authors.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""The Python implementation of the GRPC helloworld.Greeter server."""
from __future__ import print_function
from concurrent import futures
import grpc
import helloworld_pb2
import helloworld_pb2_grpc
import time
import blockchain
import threading
from global_variables import KNOWN_PEERS

NODE = 'A'

#from multiprocessing import Process

_ONE_DAY_IN_SECONDS = 60 * 60 * 24

# start a blockchain handler/object. Initialize mempool for this client
FULL_NODE_BLOCKCHAIN = blockchain.BlockChain()

import socket

hostname = socket.gethostname()
IPAddr = socket.gethostbyname(hostname)
print("This is %s:" % NODE," IP address", IPAddr)


class Greeter(helloworld_pb2_grpc.GreeterServicer):

    def SayHello(self, request, context):
        return helloworld_pb2.HelloReply(message='Hello, %s!' % request.name)

    def Handshake(self, request, context):
        # append the new node
        KNOWN_PEERS.append(request.addr_me)
        print(request.addr_me, "connected")
        print("all connections", KNOWN_PEERS)
        return helloworld_pb2.KnownPeers(known_peers=KNOWN_PEERS)  # return the latest list

    def NewTransactionBroadcast(self, request, context):
        # print("In the server code New TxnBroadcast: %s" % NODE)
        # check if this txn exists in mempool
        n = FULL_NODE_BLOCKCHAIN.pool.get_size()
        txns = FULL_NODE_BLOCKCHAIN.pool.get_txns(n)

        # make a list of all txn-hashes of the current txns in the mempool
        # since all these txn_hashes are unique and furthermore, a txn_hash does not include time so
        # the txn_hash will not differ if two txns are identical

        print("In %s received a txn from " % NODE, request.sending_node)
        txn_hashes = [txn.transaction_hash for txn in txns]
        if request.transaction_hash not in txn_hashes:
            # add this new txn to mempool

            FULL_NODE_BLOCKCHAIN.create_new_transaction(sender=request.sender,
                                                        recipient=request.recipient,
                                                        amount=request.input_amount,
                                                        output_amount=request.output_amount)

            print("successfully added new broadcasted txn to mempool of %s:" % NODE)
        return helloworld_pb2.Empty()

    def NewBlockBroadcast(self, request, context):
        # add to blockchain
        block_header = blockchain.Header(hash_prev_block=request.hash_prev_block,
                                         hash_merkle_root=request.hash_merkle_root,
                                         timestamp=request.timestamp,
                                         version=request.version,
                                         bits=request.bits)

        # open up the transactions from request ???
        print("++++++++++++++++++OPEN Up++++++++++++++++++++==")
        txn_list = []
        for txn in request.transactions:
            txn_list.append(FULL_NODE_BLOCKCHAIN.create_new_transaction(sender=txn.sender,
                                                                        recipient=txn.recipient,
                                                                        amount=txn.input_amount,
                                                                        output_amount=txn.output_amount))

        FULL_NODE_BLOCKCHAIN.create_new_block(curr_list_transactions=txn_list,
                                              header=block_header, block_hash=request.block_hash)
        print("Old block height", FULL_NODE_BLOCKCHAIN.chain_length())
        print("Added a block to %s" % NODE)
        print("New block height", FULL_NODE_BLOCKCHAIN.chain_length())
        return helloworld_pb2.Empty()


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    helloworld_pb2_grpc.add_GreeterServicer_to_server(Greeter(), server)
    server.add_insecure_port('[::]:55555')
    server.start()
    try:
        while True:
            time.sleep(_ONE_DAY_IN_SECONDS)
    except KeyboardInterrupt:
        server.stop(0)


class Registration:
    def __init__(self, n_version, n_time, addr_me):
        self.n_version = n_version
        self.n_time = n_time
        self.addr_me = addr_me


def register_with_dns():
    channel = grpc.insecure_channel('dns_seed:55555')
    stub = helloworld_pb2_grpc.GreeterStub(channel)

    # call the methods on the server
    response = stub.SayHello(helloworld_pb2.HelloRequest(name='you'))
    latest_reg_node = stub.Register(
        helloworld_pb2.RegisterRequest(n_version=1, n_time=str(int(time.time())), addr_me=IPAddr))
    print("Greeter client received: " + response.message)

    return latest_reg_node


"""
Do handshake with all other nodes
"""


def handshake(latest_reg_node):
    # Call handshake for others
    print(len(latest_reg_node.known_peers))
    if len(latest_reg_node.known_peers) == 0:
        print("This is first node %s:" % NODE)

    # only the latest node registered with DNS_SEED
    for latest_peer_ip in latest_reg_node.known_peers:
        # append this latest_reg_node
        KNOWN_PEERS.append(latest_peer_ip)

        # make a channel to connect with this node
        latest_peer_channel = grpc.insecure_channel('%s:55555' % latest_peer_ip)
        latest_peer_stub = helloworld_pb2_grpc.GreeterStub(latest_peer_channel)

        resp = latest_peer_stub.Handshake(helloworld_pb2.HandshakeRequest(n_version=1, n_time=str(int(time.time())),
                                                                          addr_me=IPAddr,best_height=10000))

        # start handshaking other nodes with this latest node and in the network
        for peer_ip in resp.known_peers:
            # print("print peer_ip", peer_ip)
            # print("Ip addr", IPAddr)
            if peer_ip != IPAddr:  # don't handshake with the oneself
                # print("IN the if:0", peer_ip, IPAddr)
                peer_channel = grpc.insecure_channel('%s:55555' % peer_ip)
                peer_stub = helloworld_pb2_grpc.GreeterStub(peer_channel)
                peer_stub.Handshake(
                    helloworld_pb2.HandshakeRequest(n_version=1, n_time=str(int(time.time())), addr_me=IPAddr,
                                                    best_height=10000))

                # add this node to the known_peers list
                KNOWN_PEERS.append(peer_ip)


def run():
    latest_reg_node = register_with_dns()
    handshake(latest_reg_node)
    print("Known Peers", KNOWN_PEERS)


def broadcast_new_block(block):
    print("In the broadcast block func")
    print("KKNONW_PEERS", KNOWN_PEERS)
    for peer in KNOWN_PEERS:
        channel = grpc.insecure_channel('%s:55555' % peer)
        stub = helloworld_pb2_grpc.GreeterStub(channel)
        print("======================================")
        print("Broadcasting block from %s to " % NODE, '%s:55555' % peer)
        print("======================================")

        # make a Block proto pbject
        block_proto = helloworld_pb2.Block()

        # open up the txns and make a separate Transaction message for proto
        txn_list = []
        for txn in block['list_of_txns']:
            # block_proto.transactions.add(0)
            txn_list.append(helloworld_pb2.Transaction(sender=txn.list_of_inputs['sender'],
                                                       recipient=txn.list_of_inputs['recipient'],
                                                       input_amount=txn.list_of_inputs['input_amount'],
                                                       output_amount=txn.list_of_inputs['output_amount'],
                                                       version_number=txn.version_number,
                                                       in_counter=txn.in_counter,
                                                       out_counter=txn.out_counter,
                                                       transaction_hash=txn.transaction_hash,
                                                       sending_node=NODE
                                                       ))



        print(txn_list)
        # resp = stub.NewBlockBroadcast(helloworld_pb2.Block(block_hash=str(block['block_hash']),
        #                                                    hash_prev_block=str(block['header'].hash_prev_block),
        #                                                    hash_merkle_root=str(block['header'].hash_merkle_root),
        #                                                    version=block['header'].version,
        #                                                    timestamp=block['header'].timestamp,
        #                                                    bits=block['header'].bits,
        #                                                    nonce=block['header'].nonce))

        resp = stub.NewBlockBroadcast(helloworld_pb2.Block(block_hash=str(block['block_hash']),
                                                    hash_prev_block=str(block['header'].hash_prev_block),
                                                    hash_merkle_root=str(block['header'].hash_merkle_root),
                                                    version=block['header'].version,
                                                    timestamp=block['header'].timestamp,
                                                    bits=block['header'].bits,
                                                    nonce=block['header'].nonce))
        print(resp)


def broadcast_new_txn(new_txn):
    print("KNOWN_PEERS in txn", KNOWN_PEERS)
    for peer in KNOWN_PEERS:
        channel = grpc.insecure_channel('%s:55555' % peer)
        stub = helloworld_pb2_grpc.GreeterStub(channel)
        stub.NewTransactionBroadcast(helloworld_pb2.Transaction(sender=new_txn.list_of_inputs['sender'],
                                                                recipient=new_txn.list_of_inputs['recipient'],
                                                                input_amount=new_txn.list_of_inputs['input_amount'],
                                                                output_amount=new_txn.list_of_inputs['output_amount'],
                                                                version_number=new_txn.version_number,
                                                                in_counter=new_txn.in_counter,
                                                                out_counter=new_txn.out_counter,
                                                                transaction_hash=new_txn.transaction_hash,
                                                                sending_node=NODE
                                                                ))
        print("successfully broadcasted the txn from %s to" % NODE, '%s:55555' % peer, "\n")


def generate_new_txn():

    # preload values in mempool
    for i in range(51):
        FULL_NODE_BLOCKCHAIN.create_new_transaction(sender="sender {}".format(i+1),
                                                    recipient="recipient {}".format(i+1),
                                                    amount=i+2,
                                                    output_amount=i+1)

    # endless generation of txns
    i = 52
    while True:
        print("length of pool in %s now" % NODE, FULL_NODE_BLOCKCHAIN.pool.get_size(), "\n")
        new_txn = FULL_NODE_BLOCKCHAIN.create_new_transaction(sender="sender {}".format(i+1),
                                                              recipient="recipient {}".format(i+1),
                                                              amount=i+2,
                                                              output_amount=i+1)

        broadcast_new_txn(new_txn)
        time.sleep(3)


def mine_blockchain():
    # print("in the mine_blockchain of %s:" % NODE)
    time.sleep(5)
    while True:
        print("size of blockchain in %s in mine function:" % NODE, FULL_NODE_BLOCKCHAIN.chain_length(), "\n")
        block = FULL_NODE_BLOCKCHAIN.mine()
        broadcast_new_block(block)
        time.sleep(10)


if __name__ == '__main__':
    print("Running NODE as a server on port 55555")
    run()   # initialize this client node

    # start a new process for generation of new txns
    gen_txn_thread = threading.Thread(target=generate_new_txn)
    gen_txn_thread.start()

    print("after thread")
    mine_thread = threading.Thread(target=mine_blockchain)
    mine_thread.start()

    serve()
    gen_txn_thread.join()
    mine_thread.join()
