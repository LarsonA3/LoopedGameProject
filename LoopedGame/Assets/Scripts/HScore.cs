using UnityEngine;
using HighScore;
using TMPro;
using UnityEngine.SceneManagement;

//make sure the plyaer has this
public class HScore : MonoBehaviour
{
    public TextMeshProUGUI scoreBox;
    public TMP_InputField nameBox;
    public static string pName = "Anonymous"; //
    public static int pScore = 0;
    private string[] funNames = {"Arle Nadja", "Rulue", "Schezo Wegey", "Dark Prince"};
    private System.Random rand = new System.Random();

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

    public void ChangeName(string newName)
    {
        pName = newName;
        print(pName + " set as name for highscore");
    }

    public void IncreaseScore(int amount)
    {
        float multiplier = DifficultySettings.Selected switch
        {
            Difficulty.Easy => 1.0f,
            Difficulty.Medium => 1.15f,
            Difficulty.Hard => 1.35f,
            Difficulty.Nightmare => 1.6f,
            _ => 1.0f
        };

        pScore += Mathf.RoundToInt(amount * multiplier);
    }

    public void FinalScore() //call this before sending to end screen
    {
        if (pName == null)
        {
            //UnityEngine.Random.
            pName = funNames[rand.Next(0, 2)]; //picks a random name from the fun list of names if an empty name is submitted
        }
        HS.SubmitHighScore(this, pName, pScore);
        print($"Player: {pName}\nHigh Score: {pScore}\n");
    }

    public void ResetScore()
    {
        pScore = 0;
        //scoreBox.text = $"Score: {pScore}";
    }
}

