using UnityEngine;
using UnityEngine.Events;

public class ProcessMessage : MonoBehaviour
{
    [SerializeField] Arduino arduino;

    public UnityEvent<float> onPhoto;

    public void _Process(Message m) //Arduino envoit à PC
    {
        string txt = m.texte;

        if (txt.Contains("tft : "))
            return;

        switch (txt)
        {
            case "P": //photo
                _PrendPhoto();
                break;

            default:
                Debug.Log($"_Process({txt}) à gérer !");
                break;
        }
    }

    void Send(string txt)
    {
        arduino.SendMessage(txt);
    }

    public void _PrendPhoto()
    {
        float t_ms = Photobooth_configuration._tempsAvantPhoto * 1000;
        Send($"t{t_ms:f0}");
        onPhoto?.Invoke(t_ms);
    }


    bool boutonLed_Switch = false;
    public void _BoutonLed_Switch()
    {
        boutonLed_Switch = !boutonLed_Switch;
        if (boutonLed_Switch) _BoutonLed_ON(); else _BoutonLed_OFF();
    }

    bool photoLed_Switch = false;
    public void _PhotoLed_Switch()
    {
        photoLed_Switch = !photoLed_Switch;
        if (photoLed_Switch) _PhotoLed_ON(); else _PhotoLed_OFF();
    }

    public void _SetColorRed() { _SetRGBColor(255, 0, 0); }
    public void _SetColorGreen() { _SetRGBColor(0, 255, 0); }
    public void _SetColorBlue() { _SetRGBColor(0, 0, 255); }

    public void _SetRGBColor(Color rgb) { _SetRGBColor((byte)(255 * rgb.r), (byte)(255 * rgb.g), (byte)(255 * rgb.b)); }
    public void _SetRGBColor(byte r, byte g, byte b) { Send($"c:{r},{g},{b}"); }

    public void _BoutonLed_ON() { Send("R2"); }
    public void _BoutonLed_OFF() { Send("r2"); }
    public void _PhotoLed_ON() { Send("R1"); }
    public void _PhotoLed_OFF() { Send("r1"); }


}
