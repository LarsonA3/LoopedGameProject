using UnityEngine;

[CreateAssetMenu(fileName = "HP Buff", menuName = "Cards/HP Buff")]
public class HPBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        float amount = isRare ? 25f : 10f;

        Debug.Log("used HP buff card");

        if (UpgradeState.Instance != null)
        {
            UpgradeState.Instance.AddMaxHP(amount);
        }

        PlayerHP playerHP = player.GetComponentInChildren<PlayerHP>();

        if (playerHP != null)
        {
            playerHP.AddMaxHealth(amount);
        }
    }
}
