using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public List<CardEffect> allCards;

    [Header("UI")]
    public GameObject[] cardUIObjects;
    public TMP_Text[] cardTextSlots;

    [Header("Player")]
    public GameObject player;

    private List<CardEffect> currentRoll = new List<CardEffect>();
    private List<CardRarity> rarityRoll = new List<CardRarity>();

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
                cardUIObjects[i].SetActive(false);
                continue;
            }

            int index = Random.Range(0, pool.Count);
            CardEffect selected = pool[index];
            pool.RemoveAt(index);

            currentRoll.Add(selected);

            CardRarity rarity = RollRarity();
            rarityRoll.Add(rarity);

            if (i < cardTextSlots.Length && cardTextSlots[i] != null)
            {
                string displayName = string.IsNullOrEmpty(selected.cardName)
                    ? selected.name
                    : selected.cardName;

                cardTextSlots[i].text =
                    displayName +
                    "\n" + rarity +
                    "\n" + selected.description;
            }

            Image btnImg = cardUIObjects[i].GetComponent<Image>();

            if (btnImg != null)
            {
                btnImg.color = GetRarityColor(rarity);
            }

            cardUIObjects[i].SetActive(true);
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

    private CardRarity RollRarity()
    {
        float roll = Random.value;

        if (roll < 0.60f)
        {
            return CardRarity.Common;
        }
        else if (roll < 0.85f)
        {
            return CardRarity.Rare;
        }
        else if (roll < 0.95f)
        {
            return CardRarity.Epic;
        }
        else
        {
            return CardRarity.Legendary;
        }
    }

    private Color GetRarityColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                return Color.white;

            case CardRarity.Rare:
                return Color.cyan;

            case CardRarity.Epic:
                return new Color(0.65f, 0.25f, 1f);

            case CardRarity.Legendary:
                return new Color(1f, 0.55f, 0f);

            default:
                return Color.white;
        }
    }
}
