using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// --- Runtime Script ---
// This class handles the walking animation
public class RobotAnimator : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float armAngle = 30f;
    public float legAngle = 30f;

    private Transform leftArm;
    private Transform rightArm;
    private Transform leftLeg;
    private Transform rightLeg;

    void Start()
    {
        // Find the limbs by name
        leftArm = transform.Find("LeftArm");
        rightArm = transform.Find("RightArm");
        leftLeg = transform.Find("LeftLeg");
        rightLeg = transform.Find("RightLeg");
    }

    void Update()
    {
        // Simple Sine wave for back-and-forth motion
        float move = Mathf.Sin(Time.time * walkSpeed);

        // Rotate Arms (Opposite to legs usually)
        if (leftArm) 
            leftArm.localRotation = Quaternion.Euler(move * armAngle, 0, 0);
        if (rightArm) 
            rightArm.localRotation = Quaternion.Euler(-move * armAngle, 0, 0);

        // Rotate Legs (Opposite to arms)
        if (leftLeg) 
            leftLeg.localRotation = Quaternion.Euler(-move * legAngle, 0, 0);
        if (rightLeg) 
            rightLeg.localRotation = Quaternion.Euler(move * legAngle, 0, 0);
    }
}

// --- Editor Script ---
#if UNITY_EDITOR
public class RobotBuilder : EditorWindow
{
    [MenuItem("Tools/Build Robot")]
    public static void BuildRobot()
    {
        // 1. Create Parent
        GameObject robotRoot = new GameObject("BipedRobot");
        Undo.RegisterCreatedObjectUndo(robotRoot, "Create Robot");

        // 2. Create Torso
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.name = "Torso";
        torso.transform.SetParent(robotRoot.transform);
        torso.transform.localPosition = Vector3.zero;
        torso.transform.localScale = new Vector3(1f, 1.5f, 0.5f);

        // 3. Create Head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(robotRoot.transform);
        head.transform.localPosition = new Vector3(0, 1.25f, 0); // Above torso
        head.transform.localScale = Vector3.one * 0.75f;

        // 4. Create Arms
        CreateLimb("LeftArm", new Vector3(-0.9f, 0.5f, 0), robotRoot.transform);
        CreateLimb("RightArm", new Vector3(0.9f, 0.5f, 0), robotRoot.transform);

        // 5. Create Legs
        // Note: Creating them lower so they look like legs. 
        CreateLimb("LeftLeg", new Vector3(-0.3f, -1.25f, 0), robotRoot.transform);
        CreateLimb("RightLeg", new Vector3(0.3f, -1.25f, 0), robotRoot.transform);

        // 6. Add Animator Script
        robotRoot.AddComponent<RobotAnimator>();

        // Select the new robot
        Selection.activeGameObject = robotRoot;
    }

    private static void CreateLimb(string name, Vector3 position, Transform parent)
    {
        GameObject limb = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        limb.name = name;
        limb.transform.SetParent(parent);
        limb.transform.localPosition = position;
        limb.transform.localScale = new Vector3(0.3f, 1.0f, 0.3f);
    }
}
#endif
