using System.Collections;
using UnityEngine;

public class GravityHammerWeapon : WeaponBase
{
    [Header("Ground Pound")]
    [SerializeField] private float groundPoundDamage = 40f;
    [SerializeField] private float groundPoundRadius = 5f;
    [SerializeField] private float groundPoundKnockback = 12f;
    [SerializeField] private float groundPoundCooldown = 5f;

    private bool canGroundPound = true;

    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!canGroundPound)
        {
            Debug.Log("Ground Pound is on cooldown.");
            return;
        }

        StartCoroutine(GroundPoundRoutine());
    }

    private IEnumerator GroundPoundRoutine()
    {
        canGroundPound = false;
        
        Vector3 center = AttackPoint.position;

        HitEnemiesInRadius(
            center,
            groundPoundRadius,
            groundPoundDamage,
            groundPoundKnockback
        );

        ClearProjectilesInRadius(
            center,
            groundPoundRadius
        );

        yield return new WaitForSeconds(groundPoundCooldown);

        canGroundPound = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(AttackPoint.position, groundPoundRadius);
    }
}
