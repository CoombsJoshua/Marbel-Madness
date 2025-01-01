using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public GameObject lockIcon;
    public GameObject[] stars; // Array of star GameObjects

    private int levelIndex;

    public void Configure(int levelIndex, int starCount, bool isUnlocked, System.Action<int> onClickCallback)
    {
        this.levelIndex = levelIndex;

        // Set level number
        levelText.text = $"Level {levelIndex}";

        // Set lock state
        lockIcon.SetActive(!isUnlocked);

        // Update stars
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < starCount);
        }

        // Add click listener
        GetComponent<Button>().onClick.AddListener(() => onClickCallback(levelIndex));

        // Disable button if locked
        GetComponent<Button>().interactable = isUnlocked;
    }
}
