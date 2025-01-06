using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Player Settings")]
    public float movementSpeed = 385.0f;
    public float maxVelocity = 7.5f;
    public float jumpPower = 6.0f;
    public bool displayNames = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateSettings(float movementSpeed, float maxVelocity, float jumpPower, bool displayNames)
    {
        this.movementSpeed = movementSpeed;
        this.maxVelocity = maxVelocity;
        this.jumpPower = jumpPower;
        this.displayNames = displayNames;

        Debug.Log("Updated Settings: " +
                  $"MovementSpeed={movementSpeed}, MaxVelocity={maxVelocity}, JumpPower={jumpPower}, DisplayNames={displayNames}");
    }
}
