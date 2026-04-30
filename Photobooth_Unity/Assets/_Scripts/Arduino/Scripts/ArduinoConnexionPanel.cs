using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArduinoConnexionPanel : MonoBehaviour
{
    [SerializeField] Arduino arduino;
    [SerializeField] UnityEngine.UI.Button connexion;
    [SerializeField] UnityEngine.UI.Button deconnexion;
    [SerializeField] UnityEngine.UI.Button refresh;
    [SerializeField] TMPro.TMP_Dropdown dd_COM;
    [SerializeField] TMPro.TMP_Dropdown dd_BAUDS;

    private void Awake()
    {
        _PortCOMS_Update();
        arduino.Fill_Dropdown(dd_BAUDS, new List<string>() { "9600", "115200" });

        connexion.onClick.AddListener(delegate () { arduino.PortCom_ON(); });
        deconnexion.onClick.AddListener(delegate () { arduino.PortCom_OFF(); });
        refresh.onClick.AddListener(delegate () { _PortCOMS_Update(); });

        dd_COM.onValueChanged.AddListener(delegate { SetCOM(dd_COM); });
        dd_BAUDS.onValueChanged.AddListener(delegate { SetBAUDS(dd_BAUDS); });
    }

    private void Start()
    {
        SetSavedValue("Port", dd_COM);
        SetCOM(dd_COM); // force si choix 0

        SetSavedValue("Baud", dd_BAUDS);
        SetBAUDS(dd_BAUDS);// force si choix 0
    }

    private void SetSavedValue(string v, TMP_Dropdown dd)
    {
        string t = PlayerPrefs.GetString(v);
        if (t != "")
            dd.value = dd.options.FindIndex(option => option.text == t);
    }

    public void _PortCOMS_Update()
    {
        arduino.Fill_Dropdown(dd_COM, Arduino.PortComs());
    }

    void SetCOM(TMPro.TMP_Dropdown dd)
    {
        arduino.Port = dd.captionText.text;
        PlayerPrefs.SetString("Port", arduino.Port);
    }
    void SetBAUDS(TMPro.TMP_Dropdown dd)
    {
        arduino.Baud = dd.captionText.text;
        PlayerPrefs.SetString("Baud", arduino.Baud);
    }
}
