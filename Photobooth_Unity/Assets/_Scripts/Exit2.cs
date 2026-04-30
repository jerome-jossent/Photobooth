using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Exit2 : MonoBehaviour {

    [SerializeField] Key toucheEXIT;
    void Update()
    {
        if (toucheEXIT == Key.None)
            return;

        if (Keyboard.current[toucheEXIT].wasPressedThisFrame)            
            _EXIT();
    }

    public void _EXIT()
    {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_WEBGL)
    Application.OpenURL("about:blank");
#else
    Application.Quit();
#endif
    }
}
