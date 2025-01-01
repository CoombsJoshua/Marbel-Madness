using UnityEngine;

public class MarbleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 385.0f;
    public float maxVelocity = 7.5f;
    public float jumpPower = 6.0f;
    public float finishDeceleration = 0.25f;

    [Header("References")]
    public Transform cameraTransform; // Reference to the main camera

    private Rigidbody rb;
    private bool grounded = false;
    private bool canMove = true;
    private bool levelFinish = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogError("Rigidbody not found!");
        }

        if (!cameraTransform)
        {
            Debug.LogError("Camera reference not set!");
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            Movement();
        }

        if (levelFinish)
        {
            // Gradually reduce gravity scale
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.MoveTowards(rb.linearVelocity.y, 0, finishDeceleration * Time.fixedDeltaTime), rb.linearVelocity.z);
        }

        // Clamp velocity
        Vector3 clampedVelocity = rb.linearVelocity;
        clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxVelocity, maxVelocity);
        clampedVelocity.z = Mathf.Clamp(clampedVelocity.z, -maxVelocity, maxVelocity);
        rb.linearVelocity = clampedVelocity;
    }

    void Update()
    {
        // Jump input
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump();
        }
    }

    private void Movement()
    {
        // Input for forward/backward and right/left movement
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Get camera directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten camera directions
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction
        Vector3 movementDirection = (forwardInput * cameraForward + horizontalInput * cameraRight).normalized;

        // Apply force to the marble
        rb.AddForce(movementDirection * movementSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    private void Jump()
    {
        // Apply upward force for jumping
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        // Detect if the marble is grounded
        if (collision.contacts.Length > 0)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
