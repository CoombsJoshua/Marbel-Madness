using Unity.Netcode;
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

    private Vector3 targetPosition; // Target position for the camera
    private float horizontalOffset = 0.0f;
    private float verticalOffset = 5.0f;

    private void Awake()
    {
        verticalOffset = followHeight;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            enabled = false; // Disable this script for non-clients
            return;
        }

        AssignLocalMarble(); // Try to assign the marble immediately if it's already spawned
    }

    private void FixedUpdate()
    {
        if (!marble) AssignLocalMarble();
        if (!marble) return;

        // Get mouse input for camera movement
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        // Update offsets based on input
        horizontalOffset += mouseX;
        verticalOffset = Mathf.Clamp(verticalOffset - mouseY, verticalClampMin, verticalClampMax);

        // Calculate the desired camera position
        targetPosition = marble.position
            - Quaternion.Euler(0, horizontalOffset, 0) * Vector3.forward * followDistance
            + Vector3.up * verticalOffset;
    }

    private void LateUpdate()
    {
        if (!marble) return;
        if (Player_Runtime.IsInMenu) return;

        // Smoothly move the camera to the calculated target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);

        // Always look at the marble
        transform.LookAt(marble.position);
    }

    private void AssignLocalMarble()
    {
        var localPlayerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        if (localPlayerObject != null)
        {
            var marbleMovement = localPlayerObject.GetComponent<MarbleMovement>();
            if (marbleMovement != null)
            {
                marble = marbleMovement.transform;
                Debug.Log($"Camera assigned to local marble: {marble.name}");
            }
            else
            {
                Debug.LogWarning("MarbleMovement component not found on local player object.");
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            AssignLocalMarble();
        }
    }
}
