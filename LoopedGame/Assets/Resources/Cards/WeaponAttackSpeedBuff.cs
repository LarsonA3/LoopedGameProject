using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Attack Speed Buff", menuName = "Cards/Weapon Attack Speed Buff")]
public class WeaponAttackSpeedBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 0.06f : 0.03f;

        Debug.Log("used weapon attack speed buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.ReduceWeaponAttackCooldown(amount);
        }
    }
}
