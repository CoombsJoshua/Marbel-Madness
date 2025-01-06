using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanManager : MonoBehaviour
{
    public static CameraPanManager Instance;

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

    /// <summary>
    /// Smoothly pans the camera to showcase the level.
    /// </summary>
    /// <param name="cameraTransform">The camera's transform.</param>
    /// <param name="cameraTarget">The target to focus on.</param>
public void ShowcaseLevel(Transform cameraTransform, Transform cameraTarget, List<Transform> pathPoints, float duration)
    {
        StartCoroutine(PerformRailPan(cameraTransform, cameraTarget, pathPoints, duration));
    }

    private IEnumerator PerformRailPan(Transform cameraTransform, Transform cameraTarget, List<Transform> pathPoints, float duration)
    {
        if (pathPoints == null || pathPoints.Count < 2)
        {
            Debug.LogWarning("Insufficient path points for rail pan.");
            yield break;
        }

        float segmentDuration = duration / (pathPoints.Count - 1); // Time per segment
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 startPosition = pathPoints[i].position;
            Vector3 endPosition = pathPoints[i + 1].position;

            float elapsedTime = 0f;

            while (elapsedTime < segmentDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsedTime / segmentDuration);

                // Interpolate position and rotation
                cameraTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                cameraTransform.LookAt(cameraTarget);

                yield return null;
            }
        }

        // Ensure the camera ends at the final point
        cameraTransform.position = pathPoints[pathPoints.Count - 1].position;
        cameraTransform.LookAt(cameraTarget);
    }
    private IEnumerator PerformLevelPan(Transform cameraTransform, Transform cameraTarget)
    {
        float duration = 3.0f; // Total time for the showcase pan
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;

        // Calculate end position dynamically
        Vector3 endPosition = cameraTarget.position + new Vector3(0, 8, -15); // Customize as needed
        Quaternion endRotation = Quaternion.LookRotation(cameraTarget.position - endPosition);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration); // Smooth easing

            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        // Ensure the camera ends exactly at the target position and rotation
        cameraTransform.position = endPosition;
        cameraTransform.rotation = endRotation;
    }

    /// <summary>
    /// Orbits the camera around the target in a cinematic way.
    /// </summary>
    /// <param name="cameraTransform">The camera's transform.</param>
    /// <param name="target">The target to orbit around.</param>
    public void PanAroundTarget(Transform cameraTransform, Transform target)
    {
        StartCoroutine(PerformPanAround(cameraTransform, target));
    }

    private IEnumerator PerformPanAround(Transform cameraTransform, Transform target)
    {
        float duration = 3.0f; // Total time for orbiting
        float orbitSpeed = 2.0f; // Speed of the orbit
        float orbitDistance = 10.0f; // Distance from the target

        Vector3 initialPosition = cameraTransform.position;
        Quaternion initialRotation = cameraTransform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate new position
            float angle = elapsedTime * orbitSpeed;
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0.5f, Mathf.Cos(angle)) * orbitDistance;

            cameraTransform.position = target.position + offset;

            // Smoothly look at the target
            Quaternion lookRotation = Quaternion.LookRotation(target.position - cameraTransform.position);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, lookRotation, Time.deltaTime * 5);

            yield return null;
        }

        // Reset camera position and rotation
        cameraTransform.position = initialPosition;
        cameraTransform.rotation = initialRotation;
    }

    
}
