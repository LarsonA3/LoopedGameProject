using UnityEngine;

public class CardEffectRuntimeDebug : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        CheckRuntimeHooks();
    }

    private void CheckRuntimeHooks()
    {
        bool allGood = true;

        if (UpgradeState.Instance == null)
        {
            Debug.LogError("[Card Runtime Debug] UpgradeState.Instance missing. Stat cards will not work.");
            allGood = false;
        }

        if (CardAbilityState.Instance == null)
        {
            Debug.LogError("[Card Runtime Debug] CardAbilityState.Instance missing. Ability cards will not work.");
            allGood = false;
        }

        if (player == null)
        {
            Debug.LogError("[Card Runtime Debug] Player missing or not tagged Player.");
            return;
        }

        CheckComponent<PlayerHP>("PlayerHP", ref allGood);
        CheckComponent<TopDownController>("TopDownController", ref allGood);
        CheckComponent<PlayerRareCardAbilityController>("PlayerRareCardAbilityController", ref allGood);
        CheckComponent<PlayerEpicCardAbilityController>("PlayerEpicCardAbilityController", ref allGood);
        CheckComponent<PlayerLegendaryCardAbilityController>("PlayerLegendaryCardAbilityController", ref allGood);

        Weapon weapon = player.GetComponentInChildren<Weapon>();

        if (weapon == null)
        {
            Debug.LogError("[Card Runtime Debug] Weapon missing under player. Weapon stat/damage cards will not apply.");
            allGood = false;
        }

        if (allGood)
            Debug.Log("[Card Runtime Debug] SUCCESS: Card runtime hooks look connected.");
        else
            Debug.LogError("[Card Runtime Debug] Some card effects will not take effect. See errors above.");
    }

    private void CheckComponent<T>(string name, ref bool allGood) where T : Component
    {
        if (player.GetComponent<T>() == null)
        {
            Debug.LogError("[Card Runtime Debug] " + name + " missing on player. Related cards will not take effect.");
            allGood = false;
        }
        else
        {
            Debug.Log("[Card Runtime Debug] Found " + name + ".");
        }
    }
}
