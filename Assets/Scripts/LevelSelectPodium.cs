using UnityEngine;

public class LevelSelectPodium : MonoBehaviour
{
    public GameObject levelSelectUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelSelectUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelSelectUI.SetActive(false);
        }
    }
}
