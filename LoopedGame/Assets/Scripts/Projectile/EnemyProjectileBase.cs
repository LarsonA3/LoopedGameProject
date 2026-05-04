using UnityEngine;

public class EnemyProjectileBase : MonoBehaviour
{
    [Header("Projectile Stats")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float speed = 8f;
    [SerializeField] protected float lifetime = 5f;

    [Header("Behavior")]
    [SerializeField] protected bool canBeCleared = true;
    [SerializeField] protected bool canBeReflected = true;
    [SerializeField] protected bool destroyOnHit = true;

    [Header("Layers")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected LayerMask wallLayer;

    protected Vector3 moveDirection;
    protected GameObject shooter;
    protected bool reflected;
    protected float currentDamage;

    public GameObject Shooter => shooter;
    public bool Reflected => reflected;
    public float CurrentDamage => currentDamage;

    protected virtual void Awake()
    {
        currentDamage = damage;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public virtual void Setup(Vector3 direction, GameObject newShooter)
    {
        moveDirection = direction.normalized;
        shooter = newShooter;
        currentDamage = damage;
        reflected = false;
    }

    public virtual void TryClear()
    {
        if (!canBeCleared) return;

        Destroy(gameObject);
    }

    public virtual void TryReflect(float reflectedDamageMultiplier)
    {
        if (!canBeReflected) return;

        reflected = true;
        currentDamage = damage * reflectedDamageMultiplier;

        if (shooter != null)
        {
            Vector3 directionToShooter = shooter.transform.position - transform.position;
            directionToShooter.y = 0f;

            if (directionToShooter.sqrMagnitude > 0.01f)
            {
                moveDirection = directionToShooter.normalized;
            }
            else
            {
                moveDirection = -moveDirection;
            }
        }
        else
        {
            moveDirection = -moveDirection;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!reflected)
        {
            TryHitPlayer(other);
        }
        else
        {
            TryHitEnemy(other);
        }

        TryHitWall(other);
    }

    protected virtual void TryHitPlayer(Collider other)
    {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;

        if (!hitPlayer) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(currentDamage);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void TryHitEnemy(Collider other)
    {
        bool hitEnemy = ((1 << other.gameObject.layer) & enemyLayer) != 0;

        if (!hitEnemy) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(currentDamage);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void TryHitWall(Collider other)
    {
        bool hitWall = ((1 << other.gameObject.layer) & wallLayer) != 0;

        if (hitWall)
        {
            Destroy(gameObject);
        }
    }
}
