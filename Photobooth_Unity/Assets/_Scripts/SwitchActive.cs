using UnityEngine;

public class SwitchActive : MonoBehaviour
{
    [SerializeField] GameObject gameobject_to_switch;

    public void _Switch()
    {
        gameobject_to_switch.SetActive(!gameobject_to_switch.activeSelf);
    }
}
