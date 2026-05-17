using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHP : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    public GameObject healthPickup;
    private static GameObject healthPickupTemplate;
    private float startingHealth;
    public bool isFinal = false;

    [Header("Enemy Type")]
    public bool isBoss;
    public bool IsBoss => isBoss;

    private bool isDead;

    public GameObject hitParticlePrefab;

    public float CurrentHP => currentHealth;
    public float MaxHP => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        startingHealth = maxHealth;
        currentHealth = maxHealth;
        isDead = false;

        hitParticlePrefab = Resources.Load<GameObject>("Particles/hitParticleENEMY");

        if (hitParticlePrefab == null)
            Debug.LogWarning("[EnemyHP] Could not find Resources/Particles/hitParticleENEMY");

       
    }

    private void OnEnable()
    {
        if (currentHealth <= 0f)
        {
            currentHealth = maxHealth;
        }

        isDead = false;
    }
    private void Start()
    {
        if (healthPickupTemplate == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Health"))
            {
                if (obj.transform.parent != null && obj.transform.parent.name == "DONOTDESTROY")
                {
                    healthPickupTemplate = obj;
                    break;
                }
            }

            if (healthPickupTemplate == null)
                Debug.LogWarning("[EnemyHP] Could not find Health pickup under DONOTDESTROY.");
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(damage, null);
    }

    public void TakeDamage(float damage, GameObject source)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (hitParticlePrefab != null)
        {
            GameObject fx = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(fx, ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f);
        }
        SoundManager.PlaySound("hitHurt (1)");
        Debug.Log("[EnemyHP] " + gameObject.name + " took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);

        if (currentHealth <= 0f)
        {
            Die(source);
        }

    }

    private void Die(GameObject source)
    {
        if (healthPickup != null)
        {
            GameObject pickup = Instantiate(healthPickup, transform.position, transform.rotation);
            Debug.Log("[EnemyHP] Spawned pickup: " + pickup.name + " at " + transform.position);
        }
        else
        {
            Debug.LogWarning("[EnemyHP] healthPickup is null on " + gameObject.name + " — nothing to spawn.");
        }   

        if (isDead)
        {
            return;
        }

        isDead = true;
        currentHealth = 0f;

        NotifyCardControllers();

        Debug.Log("[EnemyHP] " + gameObject.name + " died.");

        if (healthPickupTemplate != null)
            Instantiate(healthPickupTemplate, transform.position, transform.rotation);

        HScore.pScore += (int)startingHealth * 5;

        if (isFinal)
        {
            HScore hScore = FindObjectOfType<HScore>();
            if (hScore != null) hScore.FinalScore();
            SceneManager.LoadScene("WinGame");
            return;
        }

        Destroy(gameObject);
    }

    private void NotifyCardControllers()
    {
        PlayerRareCardAbilityController rareCards = FindObjectOfType<PlayerRareCardAbilityController>();

        if (rareCards != null)
        {
            rareCards.OnEnemyKilled(this);
        }

        PlayerEpicCardAbilityController epicCards = FindObjectOfType<PlayerEpicCardAbilityController>();

        if (epicCards != null)
        {
            epicCards.OnEnemyKilled(this);
        }

        PlayerLegendaryCardAbilityController legendaryCards = FindObjectOfType<PlayerLegendaryCardAbilityController>();

        if (legendaryCards != null)
        {
            legendaryCards.OnEnemyKilled(this);
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;

        gameObject.SetActive(true);

        Debug.Log("[EnemyHP] Reset health for " + gameObject.name);
    }

    public float GetHealthPercent()
    {
        if (maxHealth <= 0f)
        {
            return 0f;
        }

        return currentHealth / maxHealth;
    }

    public bool IsBelowHealthPercent(float percent)
    {
        return GetHealthPercent() <= percent;
    }

    public void StunFor(float duration)
    {
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();

        if (patrol == null)
        {
            patrol = GetComponentInParent<EnemyPatrol>();
        }

        if (patrol == null)
        {
            patrol = GetComponentInChildren<EnemyPatrol>();
        }

        if (patrol != null)
        {
            patrol.StunFor(duration);
        }
    }

    public void KnockbackFrom(Vector3 sourcePosition, float force)
    {
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();

        if (patrol == null)
        {
            patrol = GetComponentInParent<EnemyPatrol>();
        }

        if (patrol == null)
        {
            patrol = GetComponentInChildren<EnemyPatrol>();
        }

        if (patrol != null)
        {
            Vector3 direction = transform.position - sourcePosition;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = transform.forward;
            }

            direction.Normalize();

            patrol.TakeKnockback(direction, force);
        }
    }

    public void SlowFor(float slowPercent, float duration)
    {
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();

        if (patrol == null)
        {
            patrol = GetComponentInParent<EnemyPatrol>();
        }

        if (patrol == null)
        {
            patrol = GetComponentInChildren<EnemyPatrol>();
        }

        if (patrol != null)
        {
            patrol.SlowFor(slowPercent, duration);
        }
    }
}
