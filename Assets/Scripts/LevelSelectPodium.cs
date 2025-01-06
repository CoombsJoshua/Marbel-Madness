using UnityEngine;

public class LevelSelectPodium : MonoBehaviour
{
    public GameObject levelSelectUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Userinterface.Instance.OpenUI(OriginLabs.MenuType.LevelSelect);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelSelectUI.SetActive(false);
            Userinterface.Instance.OpenUI(OriginLabs.MenuType.None);
        }
    }
}
