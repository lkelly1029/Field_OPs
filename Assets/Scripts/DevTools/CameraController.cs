using UnityEngine;
using UnityEngine.InputSystem;

namespace SovereignState.Unity.DevTools
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 20f; // Increased default speed
        public float zoomSpeed = 2f;
        public float minHeight = 5f;
        public float maxHeight = 50f;

        private void Update()
        {
            if (Keyboard.current == null) return;

            // 1. Panning (WASD / Arrows)
            Vector3 moveDir = Vector3.zero;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveDir.z += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveDir.z -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveDir.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveDir.x += 1;

            if (moveDir != Vector3.zero)
            {
                float speed = moveSpeed;
                if (Keyboard.current.leftShiftKey.isPressed) speed *= 2.0f;

                transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
                // Debug.Log($"Moving: {moveDir} at speed {speed}"); 
            }

            // 2. Zooming (Scroll Wheel)
            if (Mouse.current != null)
            {
                float scroll = Mouse.current.scroll.ReadValue().y;
                if (scroll != 0)
                {
                    Vector3 pos = transform.position;
                    pos.y -= scroll * zoomSpeed * 0.01f;
                    pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
                    transform.position = pos;
                }
            }
        }
    }
}
