using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    //actually this script gonna be on gui it makes more sense

    List<CardEffect> cards;
    private GameObject player; //this script passes player 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //find player ^, then find all cards in resource to poplate list of cards \/
        cards = new List<CardEffect>(Resources.LoadAll<CardEffect>("Cards"));
        print($"Loaded {cards.Count} cards frm resources");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GenerateRandomCards();
    }

    public void GenerateRandomCards()
    {
        //generate 3 random cards for player to choose from, show them to player using gui.
    }

    public void ApplyCard(CardEffect card)
    {
        //upon receiving player input this is called, and applies card effect to player.
    }


        

}
