using UnityEngine;

public class CntrlsButton : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    public void onClick()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }
}
