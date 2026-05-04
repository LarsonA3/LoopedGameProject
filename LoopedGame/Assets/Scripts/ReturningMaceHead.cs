using UnityEngine;

public class ReturningMaceHead : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 30f;
    [SerializeField] private float launchSpeed = 15f;
    [SerializeField] private float returnSpeed = 20f;
    [SerializeField] private float timeBeforeReturn = 0.5f;

    [Header("Stun")]
    [SerializeField] private float stunChance = 0.35f;
    [SerializeField] private float stunDuration = 1.5f;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;

    private Transform returnTarget;
    private Vector3 launchDirection;
    private bool returning;
    private float timer;

    public void Setup(
        Vector3 direction,
        Transform target,
        float newDamage,
        float newLaunchSpeed,
        float newReturnSpeed,
        float newStunChance,
        float newStunDuration,
        LayerMask newEnemyLayer
    )
    {
        launchDirection = direction.normalized;
        returnTarget = target;

        damage = newDamage;
        launchSpeed = newLaunchSpeed;
        returnSpeed = newReturnSpeed;
        stunChance = newStunChance;
        stunDuration = newStunDuration;

        enemyLayer = newEnemyLayer;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (!returning)
        {
            transform.position += launchDirection * launchSpeed * Time.deltaTime;

            if (timer >= timeBeforeReturn)
            {
                returning = true;
            }
        }
        else
        {
            ReturnToTarget();
        }
    }

    private void ReturnToTarget()
    {
        if (returnTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 directionToTarget = returnTarget.position - transform.position;
        transform.position += directionToTarget.normalized * returnSpeed * Time.deltaTime;

        if (directionToTarget.magnitude <= 0.3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hitEnemyLayer = ((1 << other.gameObject.layer) & enemyLayer) != 0;

        if (!hitEnemyLayer) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();

        if (enemyStatus != null && !enemyStatus.IsBoss)
        {
            if (Random.value <= stunChance)
            {
                enemyStatus.TryStun(stunDuration);
            }
        }

        returning = true;
    }
}
