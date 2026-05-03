using UnityEngine;

public class BootlegLightsaberWeapon : WeaponBase
{
    [Header("Reflection")]
    [SerializeField] private float reflectedDamageMultiplier = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Attack()
    {
        base.Attack();
    }

    // Update is called once per frame
    public override void SpecialAttack()
    {
        base.SpecialAttack();
    }

    protected override void OnProjectileHitByAttack(EnemyProjectileBase projectile)
    {
        projectile.TryReflect(reflectedDamageMultiplier);
    }
}
