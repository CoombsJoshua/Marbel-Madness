using UnityEngine;

public class FloatingNameTag : MonoBehaviour
{
    public Transform target; // The ball the TextMeshPro should follow
    public Vector3 offset = new Vector3(0, 0.6f, 0); // Offset from the ball's position

    private void LateUpdate()
    {
        if (target == null) return;

        // Update position based on the ball's position + offset
        transform.position = target.position + offset;

        // Align rotation with the camera's rotation to face the player
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
