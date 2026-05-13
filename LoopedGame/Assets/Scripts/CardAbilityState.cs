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

    public int GetStacks(AbilityUpgradeType ability)
    {
        return PlayerPrefs.GetInt(GetKey(ability), 0);
    }

    public int AddStack(AbilityUpgradeType ability)
    {
        int currentStacks = GetStacks(ability);
        if (currentStacks >= MaxStacks)
        {
            return 0;
        }

        int newValue = Mathf.Min(currentStacks + 1, MaxStacks);
        PlayerPrefs.SetInt(GetKey(ability), newValue);
        return newValue;
    }

        public bool HasAbility(AbilityUpgradeType ability)
        {
            return GetStacks(ability) > 0;
        }
    
    public void ResetAbilityStacks()
    {
        foreach (AbilityUpgradeType ability in System.Enum.GetValues(typeof(AbilityUpgradeType)))
        {
            PlayerPrefs.SetInt(GetKey(ability), 0);
        }

        PlayerPrefs.Save();
    }

    private string GetKey(AbilityUpgradeType ability)
    {
        return "Ability_" + ability;
    }

}

