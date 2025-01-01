using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Configurables")]
    public float followHeight = 5.0f; // Height of the camera above the marble
    public float followDistance = 10.0f; // Distance of the camera behind the marble
    public float horizontalSpeed = 2.0f; // Speed of horizontal movement
    public float verticalSpeed = 2.0f; // Speed of vertical movement
    public float verticalClampMin = 1.0f; // Minimum height above the marble
    public float verticalClampMax = 15.0f; // Maximum height above the marble

    [Header("References")]
    public Transform marble; // Reference to the marble object

    private float horizontalOffset = 0.0f;
    private float verticalOffset = 5.0f;

    void Start()
    {
        if (!marble)
        {
            Debug.LogError("Marble reference not set!");
        }

        // Initialize vertical offset
        verticalOffset = followHeight;
    }

    void LateUpdate()
    {
        if (!marble) return;

        // Get mouse input for camera movement
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        // Update offsets based on input
        horizontalOffset += mouseX;
        verticalOffset = Mathf.Clamp(verticalOffset - mouseY, verticalClampMin, verticalClampMax);

        // Calculate the desired camera position
        Vector3 desiredPosition = marble.position
            - Quaternion.Euler(0, horizontalOffset, 0) * Vector3.forward * followDistance
            + Vector3.up * verticalOffset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);

        // Always look at the marble
        transform.LookAt(marble.position);
    }
}
