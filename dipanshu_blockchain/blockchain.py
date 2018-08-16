"""
Implement Blockchain:

Autocoins: official digital cryptocurrency of Autobots from the planet Cybertron :)
1 Autocoin = 1000 Autobits
"""
from __future__ import print_function
from hashlib import sha256
from binascii import unhexlify, hexlify
from time import time
from sys import getsizeof
from collections import deque

NODE = 'A'

def double_sha256(inp):
    return sha256(sha256(inp.encode('utf-8')).hexdigest().encode('utf-8')).hexdigest()


class TxnMemoryPool:
    def __init__(self):
        self.list_of_txns = deque()

    def add_new_txn(self, txn):
        self.list_of_txns.append(txn)

    def get_txns(self, n):
        txns = []
        for _ in range(n):
            txns.append(self.list_of_txns.popleft())
        return txns     # list of n txns from left of the queue. FIFO

    def get_size(self):
        return len(self.list_of_txns)


class Output:
    def __init__(self, value, index, script="random"):
        self.value = value/1000
        self.index = index
        self.script = script


class Transaction:
    """
    A Transaction: list_of_inputs contains the dictionary{sender, recipient, amount}
    """

    def __init__(self, list_of_inputs, in_counter=1, version_number=1, transaction_hash=None):
        self.version_number = version_number
        self.list_of_inputs = list_of_inputs
        self.list_of_outputs = Output(value=1, index=1, script="random")

        self.in_counter = in_counter
        self.out_counter = 1
        if transaction_hash:
            self.transaction_hash = transaction_hash
        else:
            self.transaction_hash = double_sha256(str(self.version_number) + str(self.list_of_inputs)
                                                  + str(self.list_of_outputs)
                                                  + str(self.in_counter) + str(self.out_counter))

    def print_transaction(self):
        print(self.list_of_outputs)


class Header:
    """
    Make a header for a block.
    """
    def __init__(self, hash_prev_block, hash_merkle_root, nonce, version=1, bits=0x1e200000, timestamp=int(time())):
        self.hash_prev_block = hash_prev_block
        self.hash_merkle_root = hash_merkle_root
        self.version = version
        self.timestamp = timestamp
        self.bits = bits
        self.nonce = nonce

    def calculate_block_hash(self):
        block_hash = double_sha256(str(self.timestamp)
                                   + str(self.hash_merkle_root)
                                   + str(self.bits)
                                   + str(self.nonce)
                                   + str(self.hash_prev_block))
        return block_hash


class Block:
    """
    Block has a header and stores transactions in a list
    """
    def __init__(self, index, block_header, transactions, block_hash):
        self.magic_number = int(0xD9B4BEF9)
        self.index = index
        self.block_header = block_header
        self.transaction_counter = len(transactions)
        self.transactions = transactions
        self.block_hash = block_hash
        self.block_size = getsizeof(self)

    def print_block(self):
        print(self.block_hash)


class BlockChain:
    """
    Implement Blockchain using Block, Transaction, and Header
    """

    # maximum number of transactions per block
    MAX_TXNS = 10

    def __init__(self):
        self.block_chain = []
        self.pool = TxnMemoryPool()
        self.reward = 50
        self.create_genesis_block()     # create genesis block on initiation


    def create_genesis_block(self):
        dummy = "".join([str(0) for _ in range(16)])
        header = Header(hash_prev_block=dummy, hash_merkle_root=dummy, nonce=0)
        block_hash = header.calculate_block_hash()
        coinbase_txn = Transaction(list_of_inputs={'output_amount': 0})
        block = Block(index=0, block_header=header, transactions=[coinbase_txn], block_hash=block_hash)
        self.block_chain.append(block)

    def create_new_transaction(self, sender=None, recipient=None, amount=None, output_amount=None):

        """return the transaction object for stub broadcasting!!!"""
        txn = Transaction(list_of_inputs={'sender': sender,
                                          'recipient': recipient,
                                          'input_amount': amount,
                                          'output_amount': output_amount})

        self.pool.add_new_txn(txn)
        return txn

    def create_new_header(self, curr_list_transactions, nonce):
        block_chain_length = len(self.block_chain)

        transactions = [txn.transaction_hash for txn in curr_list_transactions]

        hash_merkle_root = MerkleTree().merkle_tree(transactions)

        header = Header(
            hash_prev_block=self.block_chain[block_chain_length - 1].block_hash,
            hash_merkle_root=hash_merkle_root,
            nonce=nonce
        )

        return header

    def create_new_block(self, curr_list_transactions, header, block_hash):
        block_chain_length = len(self.block_chain)

        block = Block(index=block_chain_length, block_header=header, transactions=curr_list_transactions,
                      block_hash=block_hash)
        self.block_chain.append(block)
        print("block height:", block.index, "\n")

    def get_block(self, block_height=None, block_hash=None):
        for block in self.block_chain:
            if block.index == block_height or block.block_hash == block_hash:
                return block

    def get_transaction(self, transaction_hash):
        for block in self.block_chain:
            for transaction in block.transactions:
                if transaction.transaction_hash == transaction_hash:
                    return transaction

    def print_block_chain(self):
        print("printing all blocks: (height, block_hash) ")
        i = 0
        for block in self.block_chain:
            print(i, block.block_hash)
            i += 1

    def print_all_transactions(self):
        print("printing all transactions: (count, txn_hash)")
        i = 1
        for block in self.block_chain:
            for transaction in block.transactions:
                print(i, transaction.transaction_hash)
                i += 1

    def mine(self):
        print("size of pool", self.pool.get_size())

        while self.pool.get_size() != 0:
            print("mining")
            print("Length of blockchain in node %s:" % NODE, self.chain_length())
            # limit to MAX_TXNS
            if self.pool.get_size() >= self.MAX_TXNS:
                # get 9 txns from pool
                curr_list_txns = self.pool.get_txns(n=9)
            else:
                curr_list_txns = self.pool.get_txns(n=self.pool.get_size())

            # get total fee from the txns
            total_fee = 0
            for txn in curr_list_txns:
                total_fee += (txn.list_of_inputs['input_amount'] - txn.list_of_inputs['output_amount'])

            # Make a coinbase txn and add 50(reward) + total_fee into coinbase
            coinbase_txn = Transaction(list_of_inputs={'sender': "Satoshi Nakamoto's present",
                                                       'input_amount': 0,
                                                       'recipient': "miner's address",
                                                       'output_amount': self.reward + total_fee})

            # make 10 txns. Prepend coinbase txn as first txn in block
            curr_list_txns.insert(0, coinbase_txn)

            # print("number of txns in a block", len(curr_list_txns))
            # mine block
            nonce = 0
            target = 0x200000*2**(0x8*(0x1e-0x3))
            header = self.create_new_header(curr_list_txns, nonce=nonce)
            # print("target", target)
            """
            first attempt at block_hash
            """
            block_hash = header.calculate_block_hash()

            while int(block_hash, 16) >= target:
                # print(".")
                nonce += 1
                header = self.create_new_header(curr_list_txns, nonce=nonce)
                block_hash = header.calculate_block_hash()

            print("solved, nonce:", nonce)
            print("answer:   ", int(block_hash, 16))

            # successfully mined the block. create this block
            self.create_new_block(curr_list_transactions=curr_list_txns,
                                  header=header, block_hash=block_hash)


            """Only for distributed nodes.
            Delete it when running independently"""
            # print("mining")
            # print("import launch_node")
            # try:
            #     import launch_node
            # except ImportError:
            #     raise ImportError("NPO NON")
            # # broadcast this block
            # print("imported launch node")

            block = dict()
            block['list_of_txns'] = curr_list_txns
            block['header'] = header
            block['block_hash'] = block_hash
            print(block)
            return block

    def chain_length(self):
        return len(self.block_chain)

    def top_of_chain(self):
        """

        :return: top most block in the chain
        """
        return self.block_chain[self.chain_length()-1]


class MerkleTree:

    @staticmethod
    def swap_endian(inp):
        """

        :param inp: string
        :return: swapped endian byte array representation of inp
        """
        """
        Algo:
        Take a string --> convert to byte data(unhexlify) --> convert to bytearray -->
         reverse the bytes(this is Big Endian representation) --> return the hex representation of this newly
         ordered bytes(Big Endian)  
        """

        """
            represent the hexstring in byte data. 
            hexstr must contain an even number of hexadecimal digits
        """

        b_array = bytearray(unhexlify(inp))
        b_array.reverse()

        return b_array    # do the little to big endian. return the byte array for input string

    def yield_two_nodes(self, transactions, n):
        """Yield successive n-sized nodes from transactions."""
        for ind in range(0, len(transactions), n):
            yield transactions[ind: ind+n]

    def merkle_tree(self, transactions):

        """Takes an array of transactions and computes a Merkle root"""
        depth_list = []
        count = 1
        for i in self.yield_two_nodes(transactions, 2):

            if len(i) == 2:
                # convert to big endian
                left = self.swap_endian(i[0])
                right = self.swap_endian(i[1])

            else:
                left = self.swap_endian(i[0])
                right = self.swap_endian(i[0])

            count += 2
            node = left + right
            hashed_parent = self.double_hash_node(node)

            depth_list.append(hashed_parent)

        if len(depth_list) == 1:
            return depth_list[0]      # return the merkle root
        else:
            return self.merkle_tree(depth_list)

    @staticmethod
    def double_hash_node(node):
        hashed_node = sha256(sha256(node).digest()).hexdigest()  # double sha256
        hashed_node = hexlify(MerkleTree().swap_endian(hashed_node)).decode("utf-8")  # back to little endian
        return hashed_node


def main():
    # testing
    blockchain = BlockChain()
    """
    Add first 91 txns to pool
    """
    for i in range(91):
        blockchain.create_new_transaction(sender="sender {}".format(i+1),
                                          recipient="recipient {}".format(i+1),
                                          amount=i+2,
                                          output_amount=i+1)
    # start mining
    blockchain.mine()

    top_block = blockchain.top_of_chain()
    print("block height of the tip of the chain: ", top_block.index)


