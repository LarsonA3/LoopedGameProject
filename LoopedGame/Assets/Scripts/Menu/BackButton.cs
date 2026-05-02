using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    
    public void onClick()
    {
        mainMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }   
}
