using UnityEngine;

public class BouncingShieldProjectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float speed = 14f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private int maxBounces = 4;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody rb;
    private int bounceCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Setup(Vector3 direction, float newDamage, LayerMask newEnemyLayer)
    {
        damage = newDamage;
        enemyLayer = newEnemyLayer;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool hitEnemyLayer = ((1 << collision.gameObject.layer) & enemyLayer) != 0;

        if (hitEnemyLayer)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        bounceCount++;

        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
}
