using UnityEngine;

public class ParriedProjectile : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 direction;
    public float speed = 12f;
    public float lifetime = 5f;

    [Header("Damage")]
    public float baseDamage = 10f;
    public float damageMultiplier = 1f;

    [Header("Projectile Reformatting Card")]
    public bool targetNearestEnemy;
    public float homingTurnSpeed = 8f;
    public float targetSearchRadius = 30f;

    private float lifeTimer;

    private void Start()
    {
        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (targetNearestEnemy)
        {
            EnemyHP nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Vector3 targetDirection = nearestEnemy.transform.position - transform.position;
                targetDirection.y = 0f;

                if (targetDirection.sqrMagnitude > 0.001f)
                {
                    targetDirection.Normalize();

                    direction = Vector3.Slerp(
                        direction,
                        targetDirection,
                        homingTurnSpeed * Time.deltaTime
                    );

                    direction.Normalize();
                }
            }
        }

        transform.position += direction * speed * Time.deltaTime;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyHP enemy = other.GetComponentInParent<EnemyHP>();

        if (enemy == null)
        {
            return;
        }

        float finalDamage = baseDamage * damageMultiplier;

        enemy.TakeDamage(finalDamage, gameObject);

        Destroy(gameObject);
    }

    private EnemyHP FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            targetSearchRadius,
            ~0,
            QueryTriggerInteraction.Collide
        );

        EnemyHP nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
            {
                continue;
            }

            EnemyHP enemy = hit.GetComponentInParent<EnemyHP>();

            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.SqrMagnitude(enemy.transform.position - transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
