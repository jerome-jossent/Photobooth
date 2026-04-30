using UnityEngine;
using UnityEngine.UI;


// Exemple de classe pour appliquer une combinaison à un SpriteRenderer
public class SpriteColorLoader : MonoBehaviour
{
    [Header("Configuration")]
    private SpriteColorDatabase database;
    [SerializeField] private Image targetImage;
    [SerializeField] private RoundedFrame bordure;

    [Header("Options")]
    [SerializeField] private bool loadOnStart = true;

    [SerializeField] private bool change = false;

    string previous_theme_name = "";

    public static SpriteColorLoader _instance;

    private void Awake()
    {
        _instance = this;

        if (database == null)
        {
            database = Resources.Load<SpriteColorDatabase>("SpriteColorDatabase");

            if (database == null)
                Debug.LogError("SpriteColorDatabase introuvable dans Resources !");
            else
                Debug.Log("Database chargée depuis Resources !");
        }
    }

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
        Debug.Log($"[SCL] Start - database: {(database != null ? database.name : "NULL")}");
        Debug.Log($"[SCL] Combinations count: {database?.combinations?.Count ?? -1}");

        if (loadOnStart)
            LoadRandomCombination();
    }

    // Charger une combinaison aléatoire
    public void LoadRandomCombination()
    {
        if (database == null)
        {
            Debug.LogError("Database non assignée!");
            return;
        }

        // Sécurité : si une seule combinaison, on l'applique directement
        if (database.combinations.Count == 1)
        {
            ApplyCombination(database.combinations[0]);
            return;
        }

        SpriteColorCombination combo;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            combo = database.GetRandomCombination();
            attempts++;
        } while (previous_theme_name == combo.name && attempts < maxAttempts);

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

