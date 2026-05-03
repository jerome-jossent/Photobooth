using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using TMPro;
using System.Linq;
using System.Threading;
using UnityEngine.Events;

public class Arduino : MonoBehaviour
{
    public object _messagesLock = new object();
    List<Message> _messages = new List<Message>();
    int _messagesMax = 300;
    public int _messages_Count { get { return _messages.Count; } }

    Thread _readThread;
    bool _running = false;

    #region "Parametres"
    private SerialPort _mySerialPort;
    private string _port;
    private string _baud;

    private void OnDestroy()
    {
        PortCom_OFF();
    }

    internal void Fill_Dropdown(TMP_Dropdown dd, List<string> list)
    {
        if (dd == null) return;
        dd.ClearOptions();
        dd.AddOptions(list);
    }

    public SerialPort MySerialPort
    {
        get { return _mySerialPort; }
        set { _mySerialPort = value; }
    }
    public string Port
    {
        get { return _port; }
        set
        {
            if (value.ToLower().Substring(0, 2).Contains("com"))
                _port = value;
            else
                _port = "COM" + System.Text.RegularExpressions.Regex.Match(value, @"\d+").Value;
        }
    }
    public string Baud
    {
        get { return _baud; }
        set { _baud = value; }
    }

    public bool IsOpen
    {
        get
        {
            if (MySerialPort == null)
                return false;
            return MySerialPort.IsOpen;
        }
    }

    public static List<string> COM_InfoHardware { get => PortComs(); }
    #endregion

    public static List<string> PortComs()
    {
        return SerialPort.GetPortNames().ToList();
    }

    public bool PortCom_ON()
    {
        //if (Baud == "")
        //    GetBaud();

        try
        {
            //utilisation des paramètres
            MySerialPort = new SerialPort(Port, int.Parse(Baud));
            MySerialPort.ReadTimeout = 1;

            //attribution en interne des autres paramètres nécessaires
            MySerialPort.Parity = Parity.None;
            MySerialPort.StopBits = StopBits.One;
            MySerialPort.DataBits = 8;
            MySerialPort.Handshake = Handshake.None;
            MySerialPort.Open();
            Debug.Log("Connexion au port " + Port + " à " + Baud + "bps.");

            _running = true;
            _readThread = new Thread(ReadLoop);
            _readThread.IsBackground = true;
            _readThread.Start();

            SendMessage(""); //purge (premier message foiré, donc je le foire immédiatement)

            NewMessage("Connexion : ok");
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            NewMessage("Connexion : " + ex.Message);
            return false;
        }
    }

    public bool PortCom_OFF()
    {
        _running = false;

        if (MySerialPort == null) return true;
        try
        {
            MySerialPort.Close();
            Debug.Log("Déconnexion du port " + Port);
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }

    void ReadLoop() // tourne en dehors du thread Unity
    {
        while (_running && MySerialPort != null && MySerialPort.IsOpen)
        {
            try
            {
                string line = MySerialPort.ReadLine(); // bloquant, attend le \n complet
                NewMessage(line); // thread-safe grâce au lock existant
            }
            catch (TimeoutException) { }
            catch (Exception ex) { Debug.Log(ex.Message); }
        }
    }

    //void Update()
    //{
    //    ReadLine();
    //}

    //void ReadLine()
    //{
    //    if (MySerialPort != null && MySerialPort.IsOpen)
    //    {
    //        string strData = null;
    //        try
    //        {
    //            strData = MySerialPort.ReadLine(); // blocking call.
    //            NewMessage(strData);
    //        }
    //        catch (Exception ex)
    //        {
    //            switch (ex.Message)
    //            {
    //                case "The operation has timed out.":
    //                    break;

    //                default:
    //                    Debug.Log(ex.Message);
    //                    break;
    //            }
    //        }
    //    }
    //}

    void NewMessage(string strData)
    {
        Message m = new Message(strData);
        lock (_messagesLock)
        {
            _messages.Add(m);
            while (_messages.Count > _messagesMax)
                _messages.RemoveAt(0);
        }
    }

    public Message _messagePop()
    {
        Message m = null;
        lock (_messagesLock)
        {
            m = _messages[0];
            _messages.Remove(m);
        }
        return m;
    }

    public new bool SendMessage(string texte)
    {
        try
        {
            if (MySerialPort ==null || !MySerialPort.IsOpen)
                return false;

            MySerialPort?.WriteLine(texte); //write LINE, pas juste write !
            Debug.Log("SendMessage : " + texte);
            return true;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }
}