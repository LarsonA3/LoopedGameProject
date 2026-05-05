using UnityEngine;

public class CursorConfine : MonoBehaviour
{
    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}