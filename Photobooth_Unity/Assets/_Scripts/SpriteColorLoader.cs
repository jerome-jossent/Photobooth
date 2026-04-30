using UnityEngine;
using UnityEngine.UI;


// Exemple de classe pour appliquer une combinaison à un SpriteRenderer
public class SpriteColorLoader : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SpriteColorDatabase database;
    [SerializeField] private Image targetImage;
    [SerializeField] private RoundedFrame bordure;

    [Header("Options")]
    [SerializeField] private bool loadOnStart = true;

    [SerializeField] private bool change = false;

    string previous_theme_name = "";

    private void Update()
    {
        if (change)
        {
            change = false;
            LoadRandomCombination();
        }
    }


    private void Start()
    {
        if (loadOnStart)
        {
            LoadRandomCombination();
        }
    }

    // Charger une combinaison aléatoire
    public void LoadRandomCombination()
    {
        if (database == null)
        {
            Debug.LogError("Database non assignée!");
            return;
        }

        SpriteColorCombination combo;
        do
        {
            combo = database.GetRandomCombination();
        } while (previous_theme_name == combo.name);
        previous_theme_name = combo.name;
        ApplyCombination(combo);
    }

    // Appliquer une combinaison spécifique
    public void ApplyCombination(SpriteColorCombination combo)
    {
        if (combo == null || targetImage == null)
        {
            return;
        }
        targetImage.sprite = combo.sprite;
        bordure.SetBorderColor(combo.color);

        Debug.Log($"Combinaison appliquée: {combo.name}");
    }
}

