using UnityEngine;

public class BootlegLightsaberWeapon : WeaponBase
{
    [Header("Reflection")]
    [SerializeField] private float reflectedDamageMultiplier = 0.5f;

    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
    }

    protected override void OnProjectileHitByAttack(EnemyProjectileBase projectile)
    {
        projectile.TryReflect(reflectedDamageMultiplier);
    }
}
