using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MarbleMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 385.0f;
    public float maxVelocity = 7.5f;
    public float jumpPower = 6.0f;
    public float finishDeceleration = 0.25f;

    [Header("References")]
    public Transform cameraTransform; // Reference to the main camera

    [Header("Material Settings")]
    public Material marbleMaterial; // Reference to the marble's material
    public Color ownerColor = Color.white; // Color for the local player's marble
    public Color nonOwnerColor = new Color(1, 1, 1, 0.5f); // Transparent color for other players

    public NetworkVariable<int> LevelIndex = new NetworkVariable<int>(
        0, // Default value
        NetworkVariableReadPermission.Everyone, // Anyone can read
        NetworkVariableWritePermission.Owner // Only the owner can write
    );
    public TextMeshPro nameTag; // UI Text for displaying the player's username above the marble
    public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    private Rigidbody rb;
    private bool grounded = false;
    private bool canMove = true;
    private bool levelFinish = false;

      private float currentBounceForce;
    [Header("Bounce Settings")]
    public float bounceIntensity = 5.0f; // Initial bounce force
    public float bounceFalloff = 0.5f; // Multiplier to reduce bounce each time
    public AnimationCurve bounceCurve; // Custom falloff curve
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogError("Rigidbody not found!");
        }

        if (IsOwner && !cameraTransform)
        {
            cameraTransform = Camera.main.transform;
        }

        
        // Initialize bounce force
        currentBounceForce = bounceIntensity;

        // Subscribe to username updates
        Username.OnValueChanged += OnUsernameChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Userinterface.Instance.m_CanvasManager.SwitchCanvas(OriginLabs.MenuType.GameUI);
            cameraTransform = Camera.main.transform;
            // Fetch and set the Unity Services username as FixedString64Bytes
            string unityUsername = Unity.Services.Authentication.AuthenticationService.Instance.PlayerName;
                            string username = PlayerPrefs.GetString("PlayerUsername", "Player");
            Debug.Log(unityUsername);
            SetUsername(username);
            if (!cameraTransform)
            {
                Debug.LogError("No Camera.main found in the scene!");
            }
        }

        if (!marbleMaterial)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                marbleMaterial = renderer.material;
            }
        }


        UpdateMaterialTransparency();

        // Listen for changes to LevelIndex
        LevelIndex.OnValueChanged += OnLevelIndexChanged;


        // Initialize name tag
        if (nameTag != null)
        {
            nameTag.text = Username.Value.ToString();
        }

  
    }

    private void OnDestroy()
    {
        LevelIndex.OnValueChanged -= OnLevelIndexChanged;

        Username.OnValueChanged -= OnUsernameChanged;
    }

    private void OnUsernameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
       Debug.Log($"Username changed from {oldValue} to {newValue}");

        // Update the name tag
        if (nameTag != null)
        {
            nameTag.text = newValue.ToString();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUsernameServerRpc(string newUsername)
    {
        if (IsServer)
        {
            Username.Value = newUsername; // Update the username on the server
        }
    }

    public void SetUsername(string newUsername)
    {
        if (IsOwner)
        {
            // Set the username on the local client and request the server to sync it
            Username.Value = newUsername;
            SetUsernameServerRpc(newUsername);
        }
    }

    private void OnLevelIndexChanged(int oldValue, int newValue)
    {
        Debug.Log($"LevelIndex changed from {oldValue} to {newValue}.");
    }

    private void UpdateMaterialTransparency()
    {
        if (!marbleMaterial)
        {
            Debug.LogWarning("No material found on the marble!");
            return;
        }

        if (IsOwner)
        {
            marbleMaterial.SetColor("_Base_Color", ownerColor);
        }
        else
        {
            marbleMaterial.SetColor("_Base_Color", nonOwnerColor);
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner || !canMove) return;

        Movement();

        if (levelFinish)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = Mathf.MoveTowards(vel.y, 0f, finishDeceleration * Time.fixedDeltaTime);
            rb.linearVelocity = vel;
        }

        Vector3 clampedVelocity = rb.linearVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxVelocity, maxVelocity);
        clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -maxVelocity, maxVelocity);
        rb.linearVelocity = clampedVelocity;
    }

    void Update()
    {
        if (!IsOwner || !canMove) return;

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump();
        }

    // Apply settings dynamically
    ApplySettingsFromManager();

    }


private void ApplySettingsFromManager()
{
    if (!SettingsManager.Instance) return;

    // Fetch settings from the SettingsManager
    movementSpeed = SettingsManager.Instance.movementSpeed;
    maxVelocity = SettingsManager.Instance.maxVelocity;
    jumpPower = SettingsManager.Instance.jumpPower;

    // Update the visibility of nameTag
    if (nameTag != null)
    {
        nameTag.gameObject.SetActive(SettingsManager.Instance.displayNames);
    }
}

    private void Movement()
    {
        float f_input = Input.GetAxisRaw("Vertical");
        float h_input = Input.GetAxisRaw("Horizontal");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 movementDirection = (f_input * camForward + h_input * camRight).normalized;

        rb.AddForce(movementDirection * movementSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

private void Bounce()
{
    if (grounded)
    {
        // Calculate the bounce force using the curve and current bounce intensity
        float normalizedBounce = bounceCurve.Evaluate(currentBounceForce / bounceIntensity);
        float finalBounceForce = currentBounceForce * normalizedBounce;

        if (finalBounceForce > 1.5f) // Only apply bounce if the force is significant
        {
            Debug.Log($"Bounce applied with force: {finalBounceForce}");
            rb.AddForce(Vector3.up * finalBounceForce, ForceMode.Impulse);

            // Reduce the bounce force for subsequent bounces
            currentBounceForce *= bounceFalloff;

            // Reset grounded to avoid multiple bounces
            grounded = false;
        }
        else
        {
            // Reset the bounce force when it's too low
            Debug.Log("Bounce force too low, resetting.");
            currentBounceForce = bounceIntensity;
        }
    }
}


    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

private void OnCollisionEnter(Collision collision)
{
    // Check if the collision is with the ground
    if (Vector3.Dot(collision.contacts[0].normal, Vector3.up) > 0.5f)
    {
        // Only bounce if the relative velocity is significant
        if (collision.relativeVelocity.magnitude > 1.0f)
        {
            Bounce();
        }

        // Mark the marble as grounded
        grounded = true;
    }
}


    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                grounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    public void StopMovement()
    {
        if (!IsOwner) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        canMove = false;
    }

    public void EnableMovement()
    {
        if (!IsOwner) return;
        canMove = true;
    }

    public void SetLevelFinish(bool value)
    {
        levelFinish = value;
    }

        private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        Debug.Log("Trigger Entered");
        if (other.CompareTag("LevelSelectPodium"))
        {
            StopMovement();
        }
        else if (other.CompareTag("Barrier"))
        {
            Debug.Log("Die Request");
            RequestTeleportServerRpc();
        } else if(other.CompareTag("StartingZone")){
            Player_Runtime.IsInSafeArea = true;
        }
        else if (other.CompareTag("LevelEnd"))
        {

        // Mark the current level as completed
            ProgressionManager.Instance.CompleteLevel(LevelIndex.Value);
            IncrementLevelIndex();
            NotifyLevelCompleteServerRpc();
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("StartingZone"))
        {
            Debug.Log("Exited StartingZone");
            Player_Runtime.IsInSafeArea = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeleportServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server received teleport request from ClientID: {rpcParams.Receive.SenderClientId}");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.TeleportToStartingPoint(LevelIndex.Value, gameObject);
            PerformTeleportClientRpc();
        }
    }

    [ClientRpc]
    private void PerformTeleportClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Client syncing teleport position...");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.TeleportToStartingPoint(LevelIndex.Value, gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyLevelCompleteServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"Server received level completion request from ClientID: {rpcParams.Receive.SenderClientId}");

        ulong clientId = rpcParams.Receive.SenderClientId;
        PerformLevelCompleteClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    private void PerformLevelCompleteClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        Debug.Log("Client handling level completion...");

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.HandleClientLevelComplete(gameObject);
        }
    }

    public void IncrementLevelIndex(){
        int nextLevelIndex = LevelIndex.Value + 1;
        Debug.Log("Next Level Is: " + nextLevelIndex);
        if(IsOwner){
            LevelIndex.Value = nextLevelIndex;
        }
    }

    public void SetLevelIndex(int value){
                if(IsOwner){
            LevelIndex.Value = value;
        }
    }
}
