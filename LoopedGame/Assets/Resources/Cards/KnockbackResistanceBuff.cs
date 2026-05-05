using UnityEngine;

[CreateAssetMenu(fileName = "Knockback Resistance Buff", menuName = "Cards/Knockback Resistance Buff")]
public class KnockbackResistanceBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 0.1f : 0.05f;

        Debug.Log("used knockback resistance buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddKnockbackResistance(amount);
        }
    }
}
