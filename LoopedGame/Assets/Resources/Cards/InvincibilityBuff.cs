using UnityEngine;

[CreateAssetMenu(fileName = "Invincibility Buff", menuName = "Cards/Invincibility Buff")]
public class InvincibilityBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 0.2f : 0.1f;

        Debug.Log("used invincibility buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddInvincibility(amount);
        }
    }
}

