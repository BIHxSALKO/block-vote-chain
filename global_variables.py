"""
This file stores the global variable: KNOWN_PEERS for both blockchain.py and launch_node.py.
We use a global file here to allow blockchain,py to access the KNOWN_PEERS which otherwise was imported
i=to a separate instance.

import launch_node createa a new copy of this module and hence new KNOWN_PEERS variable.
"""

KNOWN_PEERS = []
