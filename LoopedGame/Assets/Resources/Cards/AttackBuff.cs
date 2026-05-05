using UnityEngine;

[CreateAssetMenu(fileName = "Attack Buff", menuName = "Cards/Attack Buff")]
public class AttackBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 5f : 2f;

        Debug.Log("used attack buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddAttackDamage(amount);
        }
    }
}
