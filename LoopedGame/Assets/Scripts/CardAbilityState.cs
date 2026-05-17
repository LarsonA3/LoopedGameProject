using System.Collections.Generic;
using UnityEngine;

public class CardAbilityState : MonoBehaviour
{
    public static CardAbilityState Instance;

    private const int MaxStacks = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CardAbilityState] Multiple instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private Dictionary<AbilityUpgradeType, int> abilityStacks = new Dictionary<AbilityUpgradeType, int>();
    public int GetStacks(AbilityUpgradeType ability)
    {
        if (!abilityStacks.ContainsKey(ability))
        {
            return 0;
        }
        return abilityStacks[ability];
    }

    public int AddStack(AbilityUpgradeType ability)
    {
        int currentStacks = GetStacks(ability);
        if (currentStacks >= MaxStacks)
        {
            return 0;
        }

        int newValue = Mathf.Min(currentStacks + 1, MaxStacks);
        abilityStacks[ability] = newValue;
        return newValue;
    }

    public bool HasAbility(AbilityUpgradeType ability)
    {
        return GetStacks(ability) > 0;
    }

    public void ResetAbilityStacks()
    {
        abilityStacks.Clear();

    }
}