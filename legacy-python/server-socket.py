import socket
import sys
import json
from log import serverLogger, getAdapter
from hwmonitor import get_dynamic_info, get_static_info

# Settings
host = '127.0.0.1'
port = 16779

# Create UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((host, port))

# Print server information
print('> Server started')
print('> Server address: {}:{}'.format(host, port))

while True:
    print("Waiting")
    data, address = sock.recvfrom(1024)
    print(f"Request type '{data}' from: {address}")
    if data == b'D':
        data = get_dynamic_info()
    else:
        data = get_static_info()
    print(data)
    sock.sendto(json.dumps(data, separators=(',', ':')).encode('utf-8'), address)
    print("Sent")
    
# Close socket
sock.close()
