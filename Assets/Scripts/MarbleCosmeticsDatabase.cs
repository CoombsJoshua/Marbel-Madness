using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MarbleCosmetic
{
    public int ID; // Unique identifier for the cosmetic
    public string Name; // Name of the cosmetic
    public Texture Texture; // Texture to apply to the marble
}

[CreateAssetMenu(fileName = "MarbleCosmeticsDatabase", menuName = "Database/Marble Cosmetics")]
public class MarbleCosmeticsDatabase : ScriptableObject
{
    public List<MarbleCosmetic> Cosmetics = new List<MarbleCosmetic>();
}
