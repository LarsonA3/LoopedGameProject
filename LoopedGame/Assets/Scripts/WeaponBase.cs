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
    [SerializeField] private float attackArc = 0.4f;
    [SerializeField] private float knockbackForce = 2f;

    [Header("Projectile Interaction")]
    [SerializeField] private bool interactsWithProjectiles = true;
    [SerializeField] private float projectileInteractionRadius = 1.3f;

    [Header("References")]
    [SerializeField] private Transform attackPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask projectileLayer;

    private Transform owner;
    private bool canAttack = true;

    public string WeaponName => weaponName;

    protected Transform Owner => owner;
    protected Transform AttackPoint => attackPoint;
    protected LayerMask EnemyLayer => enemyLayer;
    protected LayerMask ProjectileLayer => projectileLayer;

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    public virtual void Attack()
    {
        if (!canAttack)
        {
            Debug.Log("[" + weaponName + "] on cooldown.");
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    public virtual void SpecialAttack()
    {
        Debug.Log("[" + weaponName + "] has no special ability.");
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (attackPoint == null)
        {
            Debug.LogWarning("[" + weaponName + "] has no AttackPoint assigned.");
            canAttack = true;
            yield break;
        }

        HitEnemiesInRadius(
            attackPoint.position,
            attackRange,
            GetFinalDamage(),
            knockbackForce
        );

        if (interactsWithProjectiles)
        {
            InteractWithProjectilesInRadius(
                attackPoint.position,
                projectileInteractionRadius
            );
        }

        yield return new WaitForSeconds(GetFinalAttackCooldown());

        canAttack = true;
    }

    protected void HitEnemiesInRadius(Vector3 center, float radius, float hitDamage, float knockback)
    {
        Collider[] enemiesHit = Physics.OverlapSphere(center, radius, enemyLayer);

        foreach (Collider enemy in enemiesHit)
        {
            if (owner != null && enemy.gameObject == owner.gameObject)
            {
                continue;
            }

            if (!IsInAttackArc(enemy.transform.position))
            {
                continue;
            }

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            if (enemyHP != null)
            {
                enemyHP.TakeDamage(hitDamage);
            }
            else
            {
                Debug.Log("[" + weaponName + "] " + enemy.gameObject.name + " has no EnemyHP.");
            }

            EnemyKnockback enemyKnockback = enemy.GetComponent<EnemyKnockback>();

            if (enemyKnockback != null && owner != null)
            {
                Vector3 knockbackDirection = enemy.transform.position - owner.position;
                knockbackDirection.y = 0f;

                enemyKnockback.ApplyKnockback(knockbackDirection, knockback);
            }

            Debug.Log("[" + weaponName + "] hit " + enemy.gameObject.name);
        }
    }

    protected void InteractWithProjectilesInRadius(Vector3 center, float radius)
    {
        Collider[] projectilesHit = Physics.OverlapSphere(center, radius, projectileLayer);

        foreach (Collider projectileCollider in projectilesHit)
        {
            if (!IsInAttackArc(projectileCollider.transform.position))
            {
                continue;
            }

            EnemyProjectileBase projectile = projectileCollider.GetComponent<EnemyProjectileBase>();

            if (projectile != null)
            {
                OnProjectileHitByAttack(projectile);
            }
        }
    }

    protected void ClearProjectilesInRadius(Vector3 center, float radius)
    {
        Collider[] projectilesHit = Physics.OverlapSphere(center, radius, projectileLayer);

        foreach (Collider projectileCollider in projectilesHit)
        {
            EnemyProjectileBase projectile = projectileCollider.GetComponent<EnemyProjectileBase>();

            if (projectile != null)
            {
                projectile.TryClear();
            }
        }
    }

    protected virtual void OnProjectileHitByAttack(EnemyProjectileBase projectile)
    {
        projectile.TryClear();
    }

    protected Vector3 GetForwardDirection()
    {
        if (owner != null)
        {
            return owner.forward;
        }

        return transform.forward;
    }

    protected float GetFinalSpecialCooldown(float baseCooldown)
    {
        float reduction = 0f;

        if (UpgradeState.Instance != null)
        {
            reduction = UpgradeState.Instance.specialCooldownReduction;
        }

        return Mathf.Max(0.1f, baseCooldown - reduction);
    }

    private float GetFinalDamage()
    {
        float bonus = 0f;

        if (UpgradeState.Instance != null)
        {
            bonus = UpgradeState.Instance.attackDamageBonus;
        }

        return damage + bonus;
    }

    private float GetFinalAttackCooldown()
    {
        float reduction = 0f;

        if (UpgradeState.Instance != null)
        {
            reduction = UpgradeState.Instance.weaponAttackCooldownReduction;
        }

        return Mathf.Max(0.05f, attackCooldown - reduction);
    }

    private bool IsInAttackArc(Vector3 targetPosition)
    {
        if (owner == null)
        {
            return true;
        }

        Vector3 directionToTarget = targetPosition - owner.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return false;
        }

        float dot = Vector3.Dot(owner.forward, directionToTarget.normalized);

        return dot >= attackArc;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, projectileInteractionRadius);
    }
}
