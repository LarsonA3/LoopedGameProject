using UnityEngine;
using HighScore;
using TMPro;

//make sure the plyaer has this
public class HScore : MonoBehaviour
{
    public TextMeshProUGUI scoreBox;
    public static string pName = "Schezo Wegey"; //provide option to change this value somewhere
    public static int pScore = 0;

    void Start()
    {
        HS.Init(this, "Looping Game (working title)");
    }

    public void IncreaseScore(int amount)
    {
        pScore += amount;
        print($"Player: {pName}\nHigh Score: {pScore}\n");
        scoreBox.text = $"Score: {pScore}";
    }


    public void FinalScore() //call this before sending to end screen
    {
        HS.SubmitHighScore(this, pName, pScore);
        print($"Player: {pName}\nHigh Score: {pScore}\n");
    }
}

