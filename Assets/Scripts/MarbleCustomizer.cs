using UnityEngine;

public class MarbleCustomizer : MonoBehaviour
{
    [Header("References")]
    public Renderer marbleRenderer; // Reference to the marble's renderer
    public MarbleCosmeticsDatabase cosmeticsDatabase; // Reference to the cosmetics database

    private int currentCosmeticID; // Tracks the currently applied cosmetic

    private void Start()
    {
        // Set a default cosmetic on start
        ApplyCosmetic(0); // Example: Apply the first cosmetic
    }

    public void ApplyCosmetic(int cosmeticID)
    {
        MarbleCosmetic cosmetic = cosmeticsDatabase.Cosmetics.Find(c => c.ID == cosmeticID);

        if (cosmetic != null)
        {
            currentCosmeticID = cosmeticID;
            marbleRenderer.material.mainTexture = cosmetic.Texture;
            Debug.Log($"Applied cosmetic: {cosmetic.Name}");
        }
        else
        {
            Debug.LogWarning($"Cosmetic with ID {cosmeticID} not found in the database.");
        }
    }

    public MarbleCosmetic GetCurrentCosmetic()
    {
        return cosmeticsDatabase.Cosmetics.Find(c => c.ID == currentCosmeticID);
    }
}
