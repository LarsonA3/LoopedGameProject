using UnityEngine;

[CreateAssetMenu(fileName = "+1 Dash Charge", menuName = "Cards/Dash Charge Buff")]
public class DashChargeBuff : CardEffect
{
    public override void Apply(GameObject player, bool isRare)
    {
        TopDownController controller = player.GetComponentInChildren<TopDownController>();

        float amount = 1; //this one will always be one - rarities have no effect. pls ignore do not change implementation.

        Debug.Log("used dash charge buff card");

        if (controller != null)
        {
            controller.addDashCharge(amount);
        }
    }
}
