using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class PartFactory
{
    [MenuItem("Tools/Create Smart Part")]
    public static void CreateSmartPart()
    {
        string partName = "SmartSegment";
        GameObject root = new GameObject(partName);
        Undo.RegisterCreatedObjectUndo(root, "Create Smart Part");

        // 1. Create Cylinder (Bone)
        GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bone.name = "Bone";
        bone.transform.SetParent(root.transform);
        
        // Standard Cylinder is 2 units high. Center is (0,0,0).
        // Let's assume we want the pivot of the "Smart Part" to be at the center or bottom?
        // Usually, for chains, pivot is at one end.
        // Let's place the Cylinder such that its bottom is at (0,0,0) (the Joint location).
        // Cylinder height 2 -> half height 1. So move up by 1.
        bone.transform.localPosition = new Vector3(0, 1f, 0);
        bone.transform.localScale = new Vector3(0.5f, 1f, 0.5f); // Thinner

        // 2. Create Sphere (Joint) at the bottom (0,0,0)
        GameObject joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        joint.name = "Joint";
        joint.transform.SetParent(root.transform);
        joint.transform.localPosition = Vector3.zero; // Pivot point
        joint.transform.localScale = Vector3.one * 0.7f; // Slightly larger than bone width

        // 3. Create Socket at the top tip
        GameObject socket = new GameObject("Socket");
        socket.transform.SetParent(root.transform);
        // Cylinder is moved up by 1, and has height extend of 1. Top is at Y=2.
        socket.transform.localPosition = new Vector3(0, 2f, 0);

        // 4. Apply Material
        Material metalMat = FindMaterial("Metal");
        if (metalMat != null)
        {
            Renderer boneRen = bone.GetComponent<Renderer>();
            Renderer jointRen = joint.GetComponent<Renderer>();
            if (boneRen) boneRen.sharedMaterial = metalMat;
            if (jointRen) jointRen.sharedMaterial = metalMat;
        }

        Selection.activeGameObject = root;
        Debug.Log($"Created {partName} with Socket at {socket.transform.position}");
    }

    private static Material FindMaterial(string matName)
    {
        string[] guids = AssetDatabase.FindAssets($"{matName} t:Material");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }
        return null;
    }
}
#endif
