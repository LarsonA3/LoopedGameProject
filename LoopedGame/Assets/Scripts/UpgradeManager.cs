using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public List<CardEffect> allCards;

    [Header("UI")]
    public GameObject[] cardUIObjects;
    public TMP_Text[] cardTextSlots;

    [Header("Player")]
    public GameObject player;

    private List<CardEffect> currentRoll = new List<CardEffect>();
    private List<bool> rarityRoll = new List<bool>();

    private void OnEnable()
    {
        if (allCards == null || allCards.Count == 0)
        {
            allCards = new List<CardEffect>(Resources.LoadAll<CardEffect>("Cards"));
            print("loaded " + allCards.Count + " card(s)");
        }

        GenerateRandomCards();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GenerateRandomCards()
    {
        if (allCards == null || allCards.Count == 0)
        {
            print("ERROR: No cards found");
            return;
        }

        currentRoll.Clear();
        rarityRoll.Clear();

        List<CardEffect> pool = new List<CardEffect>(allCards);

        for (int i = 0; i < cardUIObjects.Length; i++)
        {
            if (pool.Count == 0)
            {
                break;
            }

            int index = Random.Range(0, pool.Count);
            CardEffect selected = pool[index];

            currentRoll.Add(selected);

            bool isRare = Random.value <= 0.25f;
            rarityRoll.Add(isRare);

            if (i < cardTextSlots.Length && cardTextSlots[i] != null)
            {
                string rarityText = isRare ? "Rare" : "Common";
                cardTextSlots[i].text = selected.name + "\n" + rarityText;
            }

            Image btnImg = cardUIObjects[i].GetComponent<Image>();

            if (btnImg != null)
            {
                btnImg.color = isRare ? Color.cyan : Color.white;
            }
        }
    }

    public void SelectCard(int index)
    {
        print("button clicked: " + index);

        if (index < 0 || index >= currentRoll.Count)
        {
            return;
        }

        if (player == null)
        {
            print("ERROR: UpgradeManager has no player assigned.");
            return;
        }

        currentRoll[index].Apply(player, rarityRoll[index]);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameObject.SetActive(false);
    }
}
