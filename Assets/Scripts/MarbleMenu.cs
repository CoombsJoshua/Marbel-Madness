using UnityEngine;

public class MarbleMenu : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f; // Speed of rotation

    private bool isDragging = false;

    void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isDragging = false;
        }

        // Rotate the marble if dragging
        if (isDragging)
        {
            RotateMarble();
        }
    }

    private void RotateMarble()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Apply rotation around the Y-axis for horizontal drag and X-axis for vertical drag
        transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World); // Horizontal rotation
        transform.Rotate(Vector3.right, mouseY * rotationSpeed, Space.Self); // Vertical rotation
    }
}
