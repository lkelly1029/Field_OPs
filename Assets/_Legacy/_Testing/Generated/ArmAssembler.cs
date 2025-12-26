using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ArmAssembler
{
    [MenuItem("Tools/Build Detailed Arm")]
    public static void BuildDetailedArm()
    {
        string rootName = "DetailedRobotArm";
        GameObject rootObj = new GameObject(rootName);
        Undo.RegisterCreatedObjectUndo(rootObj, "Build Detailed Arm");

        // 1. Shoulder (Joint)
        GameObject shoulder = RoboParts.CreateJoint("Shoulder", Vector3.one * 0.5f);
        shoulder.transform.SetParent(rootObj.transform);
        shoulder.transform.localPosition = Vector3.zero;

        // 2. UpperArm (Strut)
        // Length 2, Thickness 0.2
        GameObject upperArm = RoboParts.CreateStrut("UpperArm", 2.0f, 0.2f);
        upperArm.transform.SetParent(shoulder.transform);
        // Strut pivot is center. Move it down so top is at shoulder.
        // Length 2 -> center is at -1.
        upperArm.transform.localPosition = new Vector3(0, -1.0f, 0);

        // 3. Elbow (Joint)
        GameObject elbow = RoboParts.CreateJoint("Elbow", Vector3.one * 0.4f);
        elbow.transform.SetParent(upperArm.transform);
        // Position at bottom of upper arm.
        // Since UpperArm length is 2 and center is -1 relative to shoulder, bottom is -2.
        // But relative to UpperArm (the parent), the bottom is at local Y = -1.0 (half length).
        // However, Unity Cube is 1 unit high * Scale 2 = 2 units. Extent is 1.
        // Wait, local Y -1 is correct relative to center.
        elbow.transform.localPosition = new Vector3(0, -1.0f, 0);

        // 4. Forearm (Strut)
        // Length 1.5, Thickness 0.15
        GameObject forearm = RoboParts.CreateStrut("Forearm", 1.5f, 0.15f);
        forearm.transform.SetParent(elbow.transform);
        // Pivot at elbow. Center of forearm (length 1.5) should be at -0.75
        forearm.transform.localPosition = new Vector3(0, -0.75f, 0);

        // 5. Wrist (Joint)
        GameObject wrist = RoboParts.CreateJoint("Wrist", Vector3.one * 0.3f);
        wrist.transform.SetParent(forearm.transform);
        wrist.transform.localPosition = new Vector3(0, -0.75f, 0);

        // 6. Palm (Cube - standard primitive not in RoboParts, create manually)
        GameObject palm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        palm.name = "Palm";
        palm.transform.SetParent(wrist.transform);
        palm.transform.localScale = new Vector3(0.5f, 0.5f, 0.2f);
        palm.transform.localPosition = new Vector3(0, -0.4f, 0); // Below wrist

        // 7. Fingers (3 Digits)
        float fingerSpacing = 0.15f;
        float startX = -fingerSpacing; // Center the 3 fingers

        for (int i = 0; i < 3; i++)
        {
            GameObject finger = RoboParts.CreateDigit($"Finger_{i+1}", 3); // 3 segments per finger
            finger.transform.SetParent(palm.transform);
            
            // Position at bottom of palm. Palm Y scale is 0.5, so bottom is -0.25 local.
            // Palm center is (0,0,0) relative to wrist offset. 
            // We want fingers at the "end" of the palm.
            finger.transform.localPosition = new Vector3(startX + (i * fingerSpacing), -0.25f, 0);
            
            // Rotate fingers to point down/out naturally
            // RoboParts.CreateDigit builds up. We might need to rotate 180 or just place them.
            // Assuming CreateDigit builds in +Y. If we want them hanging down, we rotate 180 X.
            finger.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }

        Selection.activeGameObject = rootObj;
        Debug.Log("Detailed Arm Built successfully!");
    }
}
#endif
