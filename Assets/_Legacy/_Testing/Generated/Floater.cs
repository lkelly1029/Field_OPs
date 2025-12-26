using UnityEngine;

public class Floater : MonoBehaviour
{
    [Tooltip("How high the object will float")]
    public float amplitude = 0.5f;
    
    [Tooltip("How fast the object will float")]
    public float frequency = 1f;

    // Store the starting position of the object
    private Vector3 startPos;

    void Start()
    {
        // Save the start position so we can float relative to it
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a Sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Update the object's position
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
