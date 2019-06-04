using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;

public class JoyConnector : MonoBehaviour
{
    private JoyAxisWriter joyAxisWriter;
    public float axis;

    private void Start()
    {
        joyAxisWriter = GetComponent<JoyAxisWriter>();
    }
    private void Update()
    {
        joyAxisWriter.Write(axis);
    }

}
