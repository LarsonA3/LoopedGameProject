using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public List<CardEffect> allCards;

    //assign inspectr
    public GameObject[] cardUIObjects;
    public TMP_Text[] cardTextSlots;
    public GameObject player;

    private List<CardEffect> currentRoll = new List<CardEffect>();
    // track rarity for the selection logic
    private List<bool> rarityRoll = new List<bool>();


    private void OnEnable()
    {

        if (allCards == null || allCards.Count == 0)
        {
            allCards = new List<CardEffect>(Resources.LoadAll<CardEffect>("Cards"));
            print("loaded " + allCards.Count);
        }

        GenerateRandomCards();

        // in case cursor breaks somehow
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GenerateRandomCards()
    {
        if (allCards.Count == 0)
        {
            print("ERROR: No cards found");
            return;
        }

        currentRoll.Clear();
        rarityRoll.Clear();
        List<CardEffect> pool = new List<CardEffect>(allCards);

        for (int i = 0; i < cardUIObjects.Length; i++)
        {
            if (pool.Count == 0) break;

            // Pick random card
            int index = Random.Range(0, pool.Count);
            CardEffect selected = pool[index];
            currentRoll.Add(selected);
            //pool.RemoveAt(index); //prevents duplicates in the same roll - mgiht keep idk

            // 25% chance for rare crd
            bool isRare = Random.value <= 0.25f;
            rarityRoll.Add(isRare);
            //isRare = true;

            // set names
            if (i < cardTextSlots.Length && cardTextSlots[i] != null)
            {
                cardTextSlots[i].text = selected.name;
            }

            // set color
            Image btnImg = cardUIObjects[i].GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = isRare ? Color.lightBlue : Color.white;
            }
        }
    }


    public void SelectCard(int index)
    {
        print("button clicked: " + index);

        if (index < currentRoll.Count)
        {
            // pass the rolled rarity
            currentRoll[index].Apply(player, rarityRoll[index]);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameObject.SetActive(false);
        }
    }
}