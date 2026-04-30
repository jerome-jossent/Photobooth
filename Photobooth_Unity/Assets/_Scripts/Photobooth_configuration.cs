using UnityEngine;
using UnityEngine.UI;

public class Photobooth_configuration : MonoBehaviour
{
    public static Photobooth_configuration instance;

    public static float _tempsAvantPhoto;

    public Slider slider_tempsAvantPhoto;

    private void Awake()
    {
        instance = this;
    }
    bool loading;

    private void Start()
    {
        loading = true;
        _tempsAvantPhoto = PlayerPrefs.GetFloat("_tempsAvantPhoto");
        slider_tempsAvantPhoto.value = _tempsAvantPhoto;
        loading = false;
    }

    public void _SetTempsAvantPhoto(float temps)
    {
        _tempsAvantPhoto = temps;
        PlayerPrefs.SetFloat("_tempsAvantPhoto", temps);
    }

}
