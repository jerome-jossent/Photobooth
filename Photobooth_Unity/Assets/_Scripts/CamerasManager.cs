using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    public Camera[] cameras;

    Camera main;
    int index;

    public bool Next, Previous;

    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera c = cameras[i];
            if (c == Camera.main)
            {
                main = c;
                index = i;
            }
        }
    }

    public void _Previous()
    {
        index--;
        if (index < 0) index = cameras.Length - 1;
        SetNewCamera(index);
    }
    public void _Next()
    {
        index++;
        if (index > cameras.Length - 1) index = 0;
        SetNewCamera(index);
    }

    float x, x_prec;

    void Update()
    {
        x = Input.GetAxis("XboxPadDG");
        if (x != x_prec)
        {
            switch (x)
            {
                case -1:
                    Previous = true;
                    break;
                case 1:
                    Next = true;
                    break;
            }
            x_prec = x;
        }

        if (Next)
        {
            Next = false;
            _Next();
        }

        if (Previous)
        {
            Previous = false;
            _Previous();
        }
    }

    void SetNewCamera(int index)
    {
        //inactive previous
        main.targetDisplay = cameras.Length;

        //active new
        main = cameras[index];
        main.targetDisplay = 0;
    }
}
