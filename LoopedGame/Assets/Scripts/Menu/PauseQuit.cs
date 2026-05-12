using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseQuit : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject hScoreObj;
    private bool isPaused;


    void Update()
    {
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Zone1") 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;
            }
            PausePlay(isPaused);
        }
    }

    private void PausePlay(bool isPaused)
    {
        switch (isPaused)
        {
            case false:
                pauseScreen.SetActive(false);
                Time.timeScale = 1;
                break;
            case true:
                pauseScreen.SetActive(true);
                Time.timeScale = 0;
                break;

        }
    }

    public void QuitGame()
    {
        print("quitting");
        Application.Quit();
    }

    public void LoseButton()
    {
        print("giving up");
        Time.timeScale = 1;
        SceneManager.LoadScene("LoseGame");
        //scene stays frozen when loaded for some reason. can't have pieces fall in place </3
        
        
    }

    public void Restart()
    {
        print("title");
        hScoreObj.SendMessage("FinalScore");
        hScoreObj.SendMessage("ResetScore");
        SceneManager.LoadScene("TitleScreen");
    }
}
