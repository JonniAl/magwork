import socket


class NetworkConnection:
    def __init__(self, ip="", port=0, sock=None):
        if sock is None:
            self.ip = ip
            self.port = port
            self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            self.sock.connect((self.ip, self.port))
        else:
            self.sock = sock

    def __del__(self):
        self.sock.close()
