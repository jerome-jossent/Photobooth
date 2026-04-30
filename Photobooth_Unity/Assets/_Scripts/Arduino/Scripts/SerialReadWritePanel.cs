using OpenCVForUnity.PhotoModule;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.VersionControl;
using UnityEngine;

public class SerialReadWritePanel : MonoBehaviour
{
    [SerializeField] Arduino arduino;
    [SerializeField] TMPro.TMP_InputField if_txtToSend;

    [SerializeField] GameObject messagesReceiver_Go_Parent;
    [SerializeField] GameObject messagesSended_Go_Parent;
    [SerializeField] int messages_max = 50;
    [SerializeField] GameObject texte_prefab;

    void Start()
    {
        if_txtToSend.onEndEdit.AddListener(delegate { _SendByUI(); });
        _SendedClear();
        _ReceivedClear();
    }

    public void _Send(string message)
    {
        Message m = new Message(message);
        _Send(m);
    }
    public void _Send(Message message)
    {
        arduino.SendMessage(message.texte);
        UpdateMessagesSended(message);
    }

    public void _SendByUI()
    {
        _Send(if_txtToSend.text);
        if_txtToSend.text = "";
    }

    private void Update()
    {
        if (arduino == null) return;
        if (arduino._messages_Count > 0)
        {
            Message m = arduino._messagePop();

            if (Process(m))
                UpdateMessagesReceivd(m, false);
            else
            {
                //message non pris en compte par l'arduino !
                UpdateMessagesReceivd(m, true);
            }
        }
    }

    private bool Process(Message m) //Arduino envoit à PC
    {
        string txt = m.texte;
        switch (txt)
        {
            case "P": //photo

                float t_ms = Photobooth_configuration._tempsAvantPhoto*1000;
                arduino.SendMessage($"t{t_ms:f0}");

                //arduino.SendMessage("r2\n"); // éteind bouton
                //arduino.SendMessage("R1\n"); // déclenche photo
                return true;

            default:
                Debug.Log($"Process({txt}) à gérer !");
                return false;
        }

        //switch (txt)
        //{
        //    case "Arret d'urgence moteurs libres":
        //        //RPC_ControlCommand._instance._SetArretUrgence(RPC_ControlCommand.TypeArretUrgence.sansmaintient);
        //        return true;

        //    case "Arret d'urgence moteurs maintenus":
        //        //RPC_ControlCommand._instance._SetArretUrgence(RPC_ControlCommand.TypeArretUrgence.avecmaintient);
        //        return true;

        //    case "Arret d'urgence acquite":
        //        //RPC_ControlCommand._instance._SetArretUrgence(RPC_ControlCommand.TypeArretUrgence.reset);
        //        return true;

        //    default:
        //        //fonctionnement nominal (retourne le temps necessaire pour avoir réalisé la commande)
        //        if (txt.Contains("t(us)"))
        //        {
        //            //Debug.Log(txt); //t(us):100024
        //            txt = txt.Replace("t(us):", "");
        //            int temps = int.Parse(txt);
        //            //RPC_ControlCommand._instance.deltaT_s = ((float)temps) / 1000000;
        //            return true;
        //        }
        //        if (txt.Contains("Message non pris en compte : "))
        //        {
        //            return false;
        //        }
        //        return false;
        //}
    }

    public void _ReceivedClear()
    {
        ClearAllChildren._ClearAllChildren(messagesReceiver_Go_Parent);
    }

    public void _SendedClear()
    {
        ClearAllChildren._ClearAllChildren(messagesSended_Go_Parent);
    }

    void UpdateMessagesReceivd(Message message, bool warning)
    {
        ClearAllChildren._ClearAllChildren(messagesReceiver_Go_Parent, messages_max);
        GameObject go_txt = Instantiate(texte_prefab, messagesReceiver_Go_Parent.transform, true);
        TMPro.TextMeshProUGUI txt = go_txt.GetComponent<TMPro.TextMeshProUGUI>();
        txt.text = message.ToString();
        if (warning)
            txt.color = Color.red;
        Debug.Log("warning");
    }

    void UpdateMessagesSended(Message message)
    {
        ClearAllChildren._ClearAllChildren(messagesSended_Go_Parent, messages_max);
        GameObject go_txt = Instantiate(texte_prefab, messagesSended_Go_Parent.transform, true);
        go_txt.GetComponent<TMPro.TextMeshProUGUI>().text = message.ToString();
    }
}