using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public DateTime dateTime;
    public string texte;

    public Message(string strData)
    {
        dateTime = DateTime.Now;
        texte = strData;
    }

    public override string ToString()
    {
        return dateTime.ToString("HH:mm:ss.fff") + " - " + texte;
    }
}
