using System;
using UnityEngine;
using UnityEngine.UI;

public class SetFloatByUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int nbr_digit_apres_virgule;

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        text.text = slider.value.ToString("F" + nbr_digit_apres_virgule);
    }

    public void _SetFloat(float value)
    {
        slider.value = value;
        Refresh();
    }
}
