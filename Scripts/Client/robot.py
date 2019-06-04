import struct
import exceptions
from network_connection import *


class Robot(NetworkConnection):
    def __init__(self, sock, robot_name):
        super().__init__(sock=sock)
        self.robot_name = robot_name
        self.joints = self.get_all_joints()

    def __getattr__(self, item):
        if item in self.joints:
            def tmp(axis):
                self.joint(item, axis)
                return self
            return tmp
        else:
            super().__getattribute__(item)

    def joint(self, joint_name, axis):
        if joint_name not in self.joints:
            raise exceptions.JointsNotExist("Joint \"" + joint_name + "\" do not exist on robot \"" +
                                            self.robot_name + "\"")
        else:
            message_bytes = bytearray([1, len(self.robot_name)]) + bytearray(self.robot_name, encoding='utf-8') + \
                            bytearray([len(joint_name)]) + bytearray(joint_name, encoding='utf-8') + bytearray(
                struct.pack("f", axis))
            self.sock.send(message_bytes)
            data = self.sock.recv(1)
            if data[0] == 101:
                raise exceptions.RobotNotExist("Robot \"" + self.robot_name + "\" do not exist on scene!")
            if data[0] == 201:
                raise exceptions.JointsNotExist("Joint \"" + joint_name + "\" do not exist on robot \"" +
                                                self.robot_name + "\"")

    def get_all_joints(self):
        message_bytes = bytearray([254]) + bytearray(self.robot_name, encoding='utf-8')
        self.sock.send(message_bytes)
        data = self.sock.recv(1024)
        byte_already_processed = 0
        data_length = len(data)
        if data_length == 1:
            if data[0] == 101:
                raise exceptions.RobotNotExist("Robot \"" + self.robot_name + "\" do not exist on scene!")
        joints_name = []
        while byte_already_processed < data_length:
            joints_name.append(
                data[byte_already_processed + 1:1 + data[byte_already_processed] + byte_already_processed].decode())
            byte_already_processed += 1 + data[byte_already_processed]
        return joints_name

