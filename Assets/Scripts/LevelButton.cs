using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public GameObject lockIcon;


    private int levelIndex;

    public void Configure(int levelIndex, bool isUnlocked, System.Action<int> onClickCallback)
    {
        this.levelIndex = levelIndex;

        // Set level number
        levelText.text = $"Level {levelIndex}";

        // Set lock state
       // lockIcon.SetActive(!isUnlocked);

        // Add click listener
        GetComponent<Button>().onClick.AddListener(() => onClickCallback(levelIndex));
        
        // Disable button if locked
        //GetComponent<Button>().interactable = isUnlocked;
    }
}
