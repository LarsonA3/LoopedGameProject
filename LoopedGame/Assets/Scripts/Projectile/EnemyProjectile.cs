using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;

    [Header("Behavior")]
    [SerializeField] private bool canBeCleared = true;
    [SerializeField] private bool destroyOnPlayerHit = true;
    [SerializeField] private bool destroyOnWallHit = true;

    private Vector3 moveDirection;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public void TryClear()
    {
        if (!canBeCleared) return;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (destroyOnPlayerHit)
            {
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Wall"))
        {
            if (destroyOnWallHit)
            {
                Destroy(gameObject);
            }
        }
    }
    public float Damage => damage;
}
