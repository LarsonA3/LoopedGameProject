using UnityEngine;

[CreateAssetMenu(fileName = "Dash Shockwave", menuName = "Cards/Epic/Dash Shockwave")]
public class DashShockwaveCard : AbilityCardEffect
{
    protected override AbilityUpgradeType AbilityType => AbilityUpgradeType.DashShockwave;

    private void OnEnable()
    {
        cardName = "Dash Shockwave";
        description =
            "Dashing releases a small shockwave that damages nearby enemies.";
    }
}
