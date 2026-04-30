using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpriteColorCombination
{
    public string name; 
    public Sprite sprite;
    public Color color = Color.white;
}

[CreateAssetMenu(fileName = "SpriteColorDatabase", menuName = "Custom/Sprite Color Database")]
public class SpriteColorDatabase : ScriptableObject
{
    [Header("Liste des combinaisons")]
    public List<SpriteColorCombination> combinations = new List<SpriteColorCombination>();

    public SpriteColorCombination GetRandomCombination()
    {
        if (combinations == null || combinations.Count == 0)
        {
            Debug.LogWarning("Aucune combinaison disponible dans la database!");
            return null;
        }

        int randomIndex = Random.Range(0, combinations.Count);
        return combinations[randomIndex];
    }

    public SpriteColorCombination GetCombinationByIndex(int index)
    {
        if (index >= 0 && index < combinations.Count)        
            return combinations[index];        

        Debug.LogWarning($"Index {index} hors limites!");
        return null;
    }

    public SpriteColorCombination GetCombinationByName(string combinationName)
    {
        foreach (var combo in combinations)        
            if (combo.name == combinationName)            
                return combo;

        Debug.LogWarning($"Combinaison '{combinationName}' introuvable!");
        return null;
    }
}
