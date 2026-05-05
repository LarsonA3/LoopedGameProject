using UnityEngine;

[CreateAssetMenu(fileName = "Special Cooldown Buff", menuName = "Cards/Special Cooldown Buff")]
public class SpecialCooldownBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 0.25f : 0.1f;

        Debug.Log("used special cooldown buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.ReduceSpecialCooldown(amount);
        }
    }
}
