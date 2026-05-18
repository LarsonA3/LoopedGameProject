using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    // on button press load menu
    public void onClick()
    {
        print("loaded main menu");
        SceneManager.LoadScene("TitleScreen");
    }
}
