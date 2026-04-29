using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void onClick()
    {
        print("quitting...");
        Application.Quit();
    }
}
