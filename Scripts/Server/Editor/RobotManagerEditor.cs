using RosSharp.RosBridgeClient;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RobotManager))]
public class RobotManagerEditor : Editor
{
    private static bool stateSettings = true;
    private static bool[] state;
    RobotManager rm;

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target ");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("All Robot Joints", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        stateSettings = EditorGUILayout.Foldout(stateSettings, "Joints Settings", true);
        GUILayout.EndHorizontal();
        if (stateSettings)
        {
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Joints Name", EditorStyles.boldLabel);
            GUILayout.Label("Joints Objects", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            for (int i = 0; i < rm.joints_name.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                state[i] = EditorGUILayout.Foldout(state[i], rm.joints_name[i], true);
                EditorGUILayout.ObjectField(rm.joints[i], typeof(Object), true, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                if (state[i])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("Max Velocity");
                    rm.joints[i].gameObject.GetComponent<JoyAxisJointTransformWriter>().MaxVelocity = EditorGUILayout.FloatField(rm.joints[i].gameObject.GetComponent<JoyAxisJointTransformWriter>().MaxVelocity, GUILayout.Width(50));
                    GUILayout.Space(150);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

            }
        }
    }

    public void OnEnable()
    {
        rm = (RobotManager)target;
        state = new bool[rm.joints_name.Count];
        for(int i = 0; i < state.Length; i++)
        {
            state[i] = false;
        }
    }
}
