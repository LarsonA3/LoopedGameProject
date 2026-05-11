using System.Collections.Generic;
using UnityEngine;

public class DashHitbox : MonoBehaviour
{
    private PlayerRareCardAbilityController rareCards;
    private TopDownController controller;

    private readonly HashSet<EnemyHP> enemiesHitThisDash = new HashSet<EnemyHP>();

    private Collider hitboxCollider;

    private void Awake()
    {
        rareCards = GetComponentInParent<PlayerRareCardAbilityController>();
        controller = GetComponentInParent<TopDownController>();
        hitboxCollider = GetComponent<Collider>();

        if (hitboxCollider == null)
        {
            Debug.LogWarning("[DashHitbox] No Collider found. Add a trigger collider to this object.");
        }
        else
        {
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
        }
    }

    public void BeginDashHitbox()
    {
        enemiesHitThisDash.Clear();

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }
    }

    public void EndDashHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }

        enemiesHitThisDash.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rareCards == null)
        {
            return;
        }

        if (controller == null)
        {
            return;
        }

        if (!controller.IsDashing)
        {
            return;
        }

        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyHP enemy = other.GetComponentInParent<EnemyHP>();

        if (enemy == null)
        {
            return;
        }

        if (enemiesHitThisDash.Contains(enemy))
        {
            return;
        }

        enemiesHitThisDash.Add(enemy);

        rareCards.OnDashHitEnemy(enemy);
    }
}
