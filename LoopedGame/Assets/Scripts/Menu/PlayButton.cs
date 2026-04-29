using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void onClick()
    {
        print("playing game...");
        SceneManager.LoadScene("Zone1");
    }
}
