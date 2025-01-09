using OriginLabs;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Menu_MarbleBag : Menu_
{
    public override MenuType MenuType => MenuType.MarbleMenu;
    public Transform cosmeticButtonContainer; // Container for cosmetic buttons
    public GameObject cosmeticButtonPrefab; // Prefab for cosmetic buttons


    void OnEnable(){
        PopulateCosmeticButtons();

    }

    private void PopulateCosmeticButtons()
    {
        foreach (var cosmetic in Userinterface.Instance.cosmeticsDatabase.Cosmetics)
        {
            GameObject buttonObject = Instantiate(cosmeticButtonPrefab, cosmeticButtonContainer);
            Button button = buttonObject.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();

            buttonText.text = cosmetic.Name;
            int cosmeticID = cosmetic.ID; // Store the ID for the button's onClick action
            button.onClick.AddListener(() => OnCosmeticButtonClicked(cosmeticID));
        }
    }

private void OnCosmeticButtonClicked(int cosmeticID)
{
    Userinterface.Instance.ApplyCosmetic(cosmeticID);

    // Notify the MarbleMovement component of the local player
    var localPlayerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
    if (localPlayerObject != null)
    {
        var marbleMovement = localPlayerObject.GetComponent<MarbleMovement>();
        if (marbleMovement != null)
        {
            marbleMovement.SetCosmetic(cosmeticID);
        }
    }
}

}
