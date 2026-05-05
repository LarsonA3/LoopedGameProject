// Make sure the player has this.
using HighScore;
using TMPro;
using UnityEngine;

public class HScore : MonoBehaviour
{
    public TextMeshProUGUI scoreBox;

    public static string pName = "Schezo Wegey"; // Provide option to change this value somewhere.
    public static int pScore = 0;

    void Start()
    {
        HS.Init(this, "Looping Game (working title)");
        UpdateScoreUI();
    }

    public void IncreaseScore(int amount)
    {
        pScore += amount;

        print($"Player: {pName}\nScore: {pScore}\n");

        UpdateScoreUI();
    }

    public void FinalScore() // Call this before sending to end screen.
    {
        HS.SubmitHighScore(this, pName, pScore);

        print($"Player: {pName}\nFinal Score: {pScore}\n");
    }

    public void ResetScore()
    {
        pScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreBox != null)
        {
            scoreBox.text = $"Score: {pScore}";
        }
    }
}
