import socket

host = 'localhost'
port = 16779

with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
    s.connect((host, port))
    s.sendall(b'S')
    data = s.recv(1024)

print('Reply:', repr(data))

input()