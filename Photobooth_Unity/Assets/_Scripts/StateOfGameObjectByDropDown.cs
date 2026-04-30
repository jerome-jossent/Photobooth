using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateOfGameObjectByDropDown : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjects;

    public void SetGameobject(int index)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            gameObjects[i]?.SetActive(i == index);
    }
}
