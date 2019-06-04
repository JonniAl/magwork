import struct
from robot import *
import exceptions
from network_connection import *


class CoreApi(NetworkConnection):
    def __init__(self, ip, port):
        super().__init__(ip, port)
        self.robots = self.get_list_robots()

    def get_list_robots(self):
        message_bytes = bytearray([255])
        self.sock.send(message_bytes)
        data = self.sock.recv(1024)
        byte_already_processed = 0
        data_length = len(data)
        robots_name = []
        while byte_already_processed < data_length:
            robots_name.append(
                data[byte_already_processed + 1:1 + data[byte_already_processed] + byte_already_processed].decode())
            byte_already_processed += 1 + data[byte_already_processed]
        return robots_name

    def send_joints_action(self, robot_name, joint_name, axis):
        message_bytes = bytearray([1, len(robot_name)]) + bytearray(robot_name, encoding='utf-8') + \
                        bytearray([len(joint_name)]) + bytearray(joint_name, encoding='utf-8') + bytearray(
            struct.pack("f", axis))
        self.sock.send(message_bytes)

    def connect(self, robot_name):
        if robot_name in self.robots:
            return Robot(self.sock, robot_name)
        else:
            raise exceptions.RobotNotExist("Robot \"" + robot_name + "\" do not exist on scene!")
