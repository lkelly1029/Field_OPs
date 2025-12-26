
using UnityEngine;

public class SciFiDoor : MonoBehaviour
{
    public float openSpeed = 2.0f;
    public float closeSpeed = 3.0f;
    public float openDistance = 3.0f;
    public Transform doorLeft;
    public Transform doorRight;

    private Vector3 initialLeftPosition;
    private Vector3 initialRightPosition;
    private Vector3 targetLeftPosition;
    private Vector3 targetRightPosition;
    private bool isOpen = false;
    private bool isOpening = false;
    private bool isClosing = false;

    void Start()
    {
        if (doorLeft == null || doorRight == null)
        {
            Debug.LogError("SciFiDoor: DoorLeft or DoorRight transforms are not assigned!");
            enabled = false; // Disable the script if required components are missing.
            return;
        }

        initialLeftPosition = doorLeft.localPosition;
        initialRightPosition = doorRight.localPosition;

        targetLeftPosition = initialLeftPosition - new Vector3(openDistance, 0, 0);  // Move left door left. Modify as needed.
        targetRightPosition = initialRightPosition + new Vector3(openDistance, 0, 0); // Move right door right. Modify as needed.
    }

    void Update()
    {
        if (isOpening)
        {
            doorLeft.localPosition = Vector3.MoveTowards(doorLeft.localPosition, targetLeftPosition, openSpeed * Time.deltaTime);
            doorRight.localPosition = Vector3.MoveTowards(doorRight.localPosition, targetRightPosition, openSpeed * Time.deltaTime);

            if (Vector3.Distance(doorLeft.localPosition, targetLeftPosition) < 0.01f &&
                Vector3.Distance(doorRight.localPosition, targetRightPosition) < 0.01f)
            {
                doorLeft.localPosition = targetLeftPosition;
                doorRight.localPosition = targetRightPosition;
                isOpening = false;
                isOpen = true;
            }
        }
        else if (isClosing)
        {
            doorLeft.localPosition = Vector3.MoveTowards(doorLeft.localPosition, initialLeftPosition, closeSpeed * Time.deltaTime);
            doorRight.localPosition = Vector3.MoveTowards(doorRight.localPosition, initialRightPosition, closeSpeed * Time.deltaTime);

            if (Vector3.Distance(doorLeft.localPosition, initialLeftPosition) < 0.01f &&
                Vector3.Distance(doorRight.localPosition, initialRightPosition) < 0.01f)
            {
                doorLeft.localPosition = initialLeftPosition;
                doorRight.localPosition = initialRightPosition;
                isClosing = false;
                isOpen = false;
            }
        }
    }

    public void OpenDoor()
    {
        if (!isOpen && !isOpening && !isClosing)
        {
            isOpening = true;
        }
    }

    public void CloseDoor()
    {
        if (isOpen && !isClosing && !isOpening)
        {
            isClosing = true;
        }
    }

    public void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}



using UnityEngine;

public class ColorPulse : MonoBehaviour
{
    public Color startColor = Color.red;
    public Color endColor = Color.blue;
    public float pulseSpeed = 1.0f;

    private Material material;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogError("ColorPulse: No renderer found on this GameObject.");
            enabled = false;
            return;
        }

        material = rend.material; // Use material instance for changing color.

        if (material == null)
        {
            Debug.LogError("ColorPulse: Failed to get material. Make sure the object has a material assigned.");
            enabled = false;
            return;
        }

        if (!material.HasProperty(BaseColor))
        {
           Debug.LogError("ColorPulse: Material does not have _BaseColor property. Ensure you are using a URP-compatible shader.");
           enabled = false;
           return;
        }

    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
        Color newColor = Color.Lerp(startColor, endColor, t);

        if (material != null)
        {
            material.SetColor(BaseColor, newColor);
        }
    }

    void OnDestroy()
    {
        // Clean up the material instance when the script is destroyed.
        if (material != null)
        {
            Destroy(material);
        }
    }
}

