using System.Collections;
using UnityEngine;

public class GravityHammerWeapon : WeaponBase
{
    [Header("Ground Pound")]
    [SerializeField] private float groundPoundDamage = 40f;
    [SerializeField] private float groundPoundRadius = 4f;
    [SerializeField] private float groundPoundKnockback = 12f;
    [SerializeField] private float groundPoundCooldown = 3f;

    private bool canGroundPound = true;

    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!canGroundPound)
        {
            Debug.Log("[Gravity Hammer] Ground pound on cooldown.");
            return;
        }

        StartCoroutine(GroundPoundRoutine());
    }

    private IEnumerator GroundPoundRoutine()
    {
        canGroundPound = false;

        Vector3 center = transform.position;

        HitEnemiesInRadius(
            center,
            groundPoundRadius,
            groundPoundDamage,
            groundPoundKnockback
        );

        ClearProjectilesInRadius(center, groundPoundRadius);

        yield return new WaitForSeconds(GetFinalSpecialCooldown(groundPoundCooldown));

        canGroundPound = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, groundPoundRadius);
    }
}
