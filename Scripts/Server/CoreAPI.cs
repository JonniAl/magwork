using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Concurrent;
using System.Text;

public class CoreAPI : MonoBehaviour
{
    private UdpClient socket;
    public int port;
    public List<RobotManager> robotsOnScene = new List<RobotManager>();
    private ConcurrentQueue<InfoFromClient> msgs = new ConcurrentQueue<InfoFromClient>();

    private struct InfoFromClient
    {
        public byte[] msg;
        public IPEndPoint source;

        public InfoFromClient(byte[] msg, IPEndPoint source)
        {
            this.msg = msg;
            this.source = source;
        } 
    }

    private void OnUdpData(IAsyncResult result)
    {
        
        //RobotManager[] robotsOnScene = state.robotsOnScene;
        //Debug.Log(robotsOnScene[0].gameObject.name);
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] message = socket.EndReceive(result, ref source);
        InfoFromClient info = new InfoFromClient(message, source);
        msgs.Enqueue(info);
        socket.BeginReceive(new AsyncCallback(OnUdpData), null);
    }

    public void UpdateRobotList()
    {
        robotsOnScene = new List<RobotManager>(FindObjectsOfType<RobotManager>());
    }

    public void RemoveRobotInList(RobotManager rm)
    {
        robotsOnScene.Remove(rm);
    }

    private void Reset()
    {
        robotsOnScene = new List<RobotManager>(FindObjectsOfType<RobotManager>());
        port = 49001;
    }

    private void MsgType1(InfoFromClient info)
    {
        //Get name of object Robot in scene
        int length_robot_name = info.msg[1];
        byte[] robot_name_array = new byte[length_robot_name];
        Array.Copy(info.msg, 2, robot_name_array, 0, length_robot_name);
        string robot_name = System.Text.Encoding.UTF8.GetString(robot_name_array);

        //Get joint on Robot
        int length_robot_joint = info.msg[length_robot_name + 2];
        byte[] robot_joint_array = new byte[length_robot_joint];
        Array.Copy(info.msg, 3 + length_robot_name, robot_joint_array, 0, length_robot_joint);
        string joint_name = System.Text.Encoding.UTF8.GetString(robot_joint_array);

        //Get Axis for joint on robot
        float axis = BitConverter.ToSingle(info.msg, info.msg.Length - 4);
        if (axis > 1)
        {
            axis = 1;
        }
        else if (axis < 1)
        {
            axis = -1;
        }

        //Add options in script JoyConnector
        RobotManager robot = robotsOnScene.Find(x => x.gameObject.name == robot_name);
        byte[] exInfo = new byte[1] { 0 };
        if (robot == null)
        {
            //Error 101 means that robot do not exist
            exInfo[0] = 101;
            socket.Send(exInfo, exInfo.Length, info.source);
            return;
        }
        if (!robot.setJointAction(joint_name, axis))
        {
            //Error 201 means that joint do not exist on robot
            exInfo[0] = 201;
            socket.Send(exInfo, exInfo.Length, info.source);
            return;
        }
         //Success
         socket.Send(exInfo, exInfo.Length, info.source);
    }

    private void MsgType255(InfoFromClient info)
    {
        List<byte> send_robot_name = new List<byte>();
        for (int i = 0; i < robotsOnScene.Count; i++)
        {
            send_robot_name.Add((byte)robotsOnScene[i].gameObject.name.Length);
            send_robot_name.AddRange(Encoding.ASCII.GetBytes(robotsOnScene[i].gameObject.name));
        }
        socket.Send(send_robot_name.ToArray(), send_robot_name.Count, info.source);
    }

    
    private void MsgType254(InfoFromClient info)
    {
        byte[] robot_name_array = new byte[info.msg.Length - 1];
        Array.Copy(info.msg, 1, robot_name_array, 0, info.msg.Length - 1);
        string robot_name = System.Text.Encoding.UTF8.GetString(robot_name_array);
        RobotManager robot = robotsOnScene.Find(x => x.gameObject.name == robot_name);
        if (robot == null)
        {
            //Error 101 means that robot do not exist
            byte[] exInfo = new byte[1] { 101 };
            socket.Send(exInfo, exInfo.Length, info.source);
            return;
        }
        List<byte> send_joints_name = new List<byte>();
        for (int i = 0; i < robot.joints_name.Count; i++)
        {
            send_joints_name.Add((byte)robot.joints_name[i].Length);
            send_joints_name.AddRange(Encoding.ASCII.GetBytes(robot.joints_name[i]));
        }
        socket.Send(send_joints_name.ToArray(), send_joints_name.Count, info.source);
    }

    private void Update()
    {
        if (!msgs.IsEmpty)
        {
            InfoFromClient info;
            msgs.TryDequeue(out info);
            switch (info.msg[0])
            {
                //Add Axis in joint on robot
                case 1:
                    MsgType1(info);
                    break;
                //Send info about all robots on scene to client
                case 255:
                    MsgType255(info);
                    break;

                //Send info about joints on robot to client
                case 254:
                    MsgType254(info);
                    break;

                default:
                    Debug.Log("Unknown action type: " + info.msg[0]);
                    break;
            }
        }
    }

    void Start()
    {
        if (robotsOnScene.Count == 0)
        {
            robotsOnScene = new List<RobotManager>(FindObjectsOfType<RobotManager>());
        }
        socket = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        socket.BeginReceive(new AsyncCallback(OnUdpData), null);
    }
}
