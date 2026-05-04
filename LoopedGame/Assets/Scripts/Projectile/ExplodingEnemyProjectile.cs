using UnityEngine;

public class ExplodingEnemyProjectile : EnemyProjectileBase
{
    [Header("Explosion")]
    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float reflectedExplosionDamageMultiplier = 0.5f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionKnockback = 8f;

    protected override void OnTriggerEnter(Collider other)
    {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        bool hitWall = ((1 << other.gameObject.layer) & wallLayer) != 0;
        bool hitEnemy = ((1 << other.gameObject.layer) & enemyLayer) != 0;

        if (!reflected && hitPlayer)
        {
            Explode();
        }
        else if (reflected && hitEnemy)
        {
            Explode();
        }
        else if (hitWall)
        {
            Explode();
        }
    }

    private void Explode()
    {
        LayerMask targetLayer = reflected ? enemyLayer : playerLayer;

        float finalExplosionDamage = explosionDamage;

        if (reflected)
        {
            finalExplosionDamage *= reflectedExplosionDamageMultiplier;
        }

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            targetLayer
        );

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(finalExplosionDamage);
            }

            EnemyKnockback knockback = hit.GetComponent<EnemyKnockback>();

            if (knockback != null)
            {
                Vector3 direction = hit.transform.position - transform.position;
                direction.y = 0f;

                knockback.ApplyKnockback(direction, explosionKnockback);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
