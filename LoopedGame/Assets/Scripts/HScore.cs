using UnityEngine;
using HighScore;

public class HScore : MonoBehaviour
{
    public string pName = "Schezo Wegey";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //HS.Init(this, "uh (working title)");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinalScore(int score) {
        //HS.SubmitScore(pName, score);
    }
}

