using UnityEngine;
using HighScore;
using TMPro;
using UnityEngine.SceneManagement;

//make sure the plyaer has this
public class HScore : MonoBehaviour
{
    public TextMeshProUGUI scoreBox;
    public TextMeshProUGUI nameBox;
    public static string pName = "Schezo Wegey"; //
    public static int pScore = 0;

    void Start()
    {
        HS.Init(this, "Reboot");
    }

    void Update()
    {
        var currentScene = SceneManager.GetActiveScene();
        if (scoreBox != null)
        {
            if (pScore < 0)
            {
                pScore = 0;
            }

            scoreBox.text = $"Score: {pScore}";
        }

    }

    public void ChangeName()
    {
        if (nameBox != null)
        {
            pName = nameBox.text;    
            print(pName);
        }

    }

    public void IncreaseScore(int amount)
    {
        pScore += amount;
        //print($"Player: {pName}\nHigh Score: {pScore}\n");
        //scoreBox.text = $"Score: {pScore}";
    }

    public void FinalScore() //call this before sending to end screen
    {
        HS.SubmitHighScore(this, pName, pScore);
        print($"Player: {pName}\nHigh Score: {pScore}\n");
    }

    public void ResetScore()
    {
        pScore = 0;
        //scoreBox.text = $"Score: {pScore}";
    }
}

