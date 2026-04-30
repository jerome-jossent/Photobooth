using UnityEngine;

public class Rotate180 : MonoBehaviour
{
    float angle;

    private void Start()
    {
        angle = transform.rotation.eulerAngles.z;
    }

    public void _Rotate180()
    {
        angle += 180;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
    }

}
