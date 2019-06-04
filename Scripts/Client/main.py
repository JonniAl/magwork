import time
from core_api import *

general_instr = CoreApi("127.0.0.1", 49001)
robots_name = general_instr.get_list_robots()

robot1 = general_instr.connect("Robot")
print(robot1.get_all_joints())

robot2 = general_instr.connect("autorally_platform")

robot2.joint("right_front_wheel_3", 1)
robot2.joint("left_front_wheel_1", 1)
robot2.joint("right_rear_wheel_5", 1)
robot2.joint("left_rear_wheel_4", 1)

#robot1.wrist_0(-1)


robot1.joint("ffknuckle_2", -1)
robot1.joint("mfknuckle_6", 1)

robot1.joint("thmiddle_22", 1)
robot1.joint("thproximal_20", 1)
robot1.joint("thhub_21", 1)
robot1.joint("thdistal_23", 1)


robot1.joint("rfproximal_11", 1)
robot1.joint("rfmiddle_12", 1)
robot1.joint("rfdistal_13", 1)

robot1.joint("lfproximal_16", 1)
robot1.joint("lfmiddle_17", 1)
robot1.joint("lfdistal_18", 1)

"""
fuck u
robot1.joint("ffmiddle_4", 1)
robot1.joint("ffdistal_5", 1)

robot1.joint("rfmiddle_12", 1)
robot1.joint("rfdistal_13", 1)

robot1.joint("lfmiddle_17", 1)
robot1.joint("lfdistal_18", 1)
"""



