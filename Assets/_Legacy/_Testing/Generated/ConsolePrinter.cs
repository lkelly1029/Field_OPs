using UnityEngine;
using System;

public class ConsolePrinter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the current system time
        string currentTime = DateTime.Now.ToString("HH:mm:ss");

        // Print the message in green text followed by the system time
        // The <color=green> tag is used for Unity console rich text formatting
        Debug.Log($"<color=green>AI Code is Working</color> - Current Time: {currentTime}");
    }
}
