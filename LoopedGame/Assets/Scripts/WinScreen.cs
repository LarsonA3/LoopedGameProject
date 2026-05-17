using UnityEngine;
using TMPro;
using HighScore;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText.text = $"Score: {HScore.pScore}";
        //HS.SubmitHighScore(FindObjectOfType<HScore>(), HScore.pName, HScore.pScore); DONT UNCOMMENT
        HScore.pScore = 0;
    }
}