using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Base Weapon";

    [Header("Weapon Stats")]
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float knockbackForce = 2f;

    [Header("Projectile Clearing")]
    [SerializeField] private bool clearsProjectiles = true;
    [SerializeField] private float projectileClearRadius = 1.3f;

    [Header("References")]
    [SerializeField] private Transform attackPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask projectileLayer;

    private Transform owner;
    private bool canAttack = true;

    public string WeaponName
    {
        get { return weaponName; }
    }

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    public virtual void Attack()
    {
        if (!canAttack) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        HitEnemies();

        if (clearsProjectiles)
        {
            ClearProjectiles();
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private void HitEnemies()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning(weaponName + " has no AttackPoint assigned.");
            return;
        }

        Collider[] enemiesHit = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemy in enemiesHit)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            EnemyKnockback knockback = enemy.GetComponent<EnemyKnockback>();

            if (knockback != null && owner != null)
            {
                Vector3 knockbackDirection = enemy.transform.position - owner.position;
                knockback.ApplyKnockback(knockbackDirection, knockbackForce);
            }
        }
    }

    private void ClearProjectiles()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning(weaponName + " has no AttackPoint assigned.");
            return;
        }

        Collider[] projectilesHit = Physics.OverlapSphere(
            attackPoint.position,
            projectileClearRadius,
            projectileLayer
        );

        foreach (Collider projectile in projectilesHit)
        {
            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

            if (enemyProjectile != null)
            {
                enemyProjectile.TryClear();
            }
            else
            {
                Destroy(projectile.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, projectileClearRadius);
    }
}
