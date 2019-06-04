using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;

[ExecuteInEditMode]
public class RobotManager : MonoBehaviour
{
    public GameObject target;
    public List<GameObject> joints = new List<GameObject>();
    public List<string> joints_name = new List<string>();
    private int defaultMaxVelocity = 20;

    private void Reset()
    {
        if (!target)
        {
            target = this.gameObject;
        }

        CoreAPI coreAPI = FindObjectOfType<CoreAPI>();

        if (coreAPI == null)
        {
            GameObject coreAPIobj = new GameObject("CoreAPI");
            CoreAPI coreComponent = coreAPIobj.AddComponent<CoreAPI>();
            coreComponent.port = 49001;
        } else
        {
            coreAPI.UpdateRobotList();
        }
        addJoyComponents(target);

    }

    private void OnDisable()
    {
        CoreAPI coreAPI = FindObjectOfType<CoreAPI>();
        if (coreAPI != null)
        {
            coreAPI.RemoveRobotInList(this);
        }
    }

    void addJoyComponents(GameObject gameObject)
    {
        if (gameObject.GetComponent<HingeJoint>() != null)
        {
            if (gameObject.GetComponent<JoyAxisJointTransformWriter>() == null)
            {
                gameObject.AddComponent<JoyAxisJointTransformWriter>().MaxVelocity = defaultMaxVelocity;
                
            }
            else
            {
                gameObject.GetComponent<JoyAxisJointTransformWriter>().MaxVelocity = defaultMaxVelocity;
            }
            if (gameObject.GetComponent<JoyConnector>() == null)
            {
                gameObject.AddComponent<JoyConnector>();
            }
            joints.Add(gameObject);
            joints_name.Add(gameObject.name + "_" + joints_name.Count);
        }
        foreach(Transform child in gameObject.transform)
        {
            addJoyComponents(child.gameObject);
        }
        
    }

    private void Update()
    {
        
    }

    public bool setJointAction(string joint_name, float axis)
    {
        int index_action = joints_name.FindIndex(x => x==joint_name);
        if (index_action == -1)
        {
            return false;
        }
        joints[index_action].GetComponent<JoyConnector>().axis = axis;
        return true;
    }
}
