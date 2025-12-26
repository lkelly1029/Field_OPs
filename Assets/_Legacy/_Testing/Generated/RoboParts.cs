using UnityEngine;

/// <summary>
/// Static utility class for generating robot parts using Unity primitives.
/// All parts generated here include their default colliders.
/// </summary>
public static class RoboParts
{
    /// <summary>
    /// Creates a sphere representing a robot joint.
    /// </summary>
    public static GameObject CreateJoint(string name, Vector3 size)
    {
        GameObject joint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        joint.name = name;
        joint.transform.localScale = size;
        return joint;
    }

    /// <summary>
    /// Creates a cube scaled to look like a metal beam or structural strut.
    /// </summary>
    public static GameObject CreateStrut(string name, float length, float thickness)
    {
        GameObject strut = GameObject.CreatePrimitive(PrimitiveType.Cube);
        strut.name = name;
        // Unity Cubes are 1x1x1. We scale Y for length and XZ for thickness.
        strut.transform.localScale = new Vector3(thickness, length, thickness);
        return strut;
    }

    /// <summary>
    /// Creates a hierarchical chain of small capsules to represent a robot finger or digit.
    /// </summary>
    public static GameObject CreateDigit(string name, int segments)
    {
        GameObject digitRoot = new GameObject(name);
        Transform parentTransform = digitRoot.transform;

        float segmentHeight = 0.2f;
        float thickness = 0.05f;

        for (int i = 0; i < segments; i++)
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            segment.name = $"{name}_Segment_{i}";
            segment.transform.SetParent(parentTransform);

            // Capsule primitive is 2 units high by default. 
            // We scale it so its total height is 'segmentHeight'.
            segment.transform.localScale = new Vector3(thickness, segmentHeight * 0.5f, thickness);

            // Offset the segment so they stack. 
            // If i=0, move it up by half its height from the root.
            // If i>0, move it up from the parent's pivot.
            float yOffset = (i == 0) ? (segmentHeight * 0.5f) : (segmentHeight);
            segment.transform.localPosition = new Vector3(0, yOffset, 0);

            // The next segment will be a child of this one to create a chain.
            parentTransform = segment.transform;
        }

        return digitRoot;
    }
}
