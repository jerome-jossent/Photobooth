using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClearAllChildren
{
    public static void _ClearAllChildren(GameObject Parent)
    {
        while (Parent.transform.childCount > 0)
            GameObject.DestroyImmediate(Parent.transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="keepLastCount">number of childs to keep (only last children will be kept)</param>
    public static void _ClearAllChildren(GameObject Parent, int keepLastCount)
    {
        while (Parent.transform.childCount > keepLastCount)
            GameObject.DestroyImmediate(Parent.transform.GetChild(0).gameObject);
    }

}
