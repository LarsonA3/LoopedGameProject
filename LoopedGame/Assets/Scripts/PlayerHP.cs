using System;
using System.Collections;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    public static float currentHealth;

    public static event Action<float, float> OnHealthChanged; // used for ui 

    [Header("Damage")]
    [SerializeField] private float baseInvincibilityTime = 0.3f;
    private bool isDead = false;
    private bool isInvincible = false;

    [Header("Layers")]
    [SerializeField] private LayerMask projectileLayer;


    [SerializeField] private float heavyHitThreshold = 5f;
    [SerializeField] private float blockMeterDrainPerDamage = 0.2f;

    private void Awake()
    {
        if (UpgradeState.Instance != null)
            maxHealth += UpgradeState.Instance.maxHPBonus;

        currentHealth = maxHealth;
        Debug.Log("[PlayerHP] Starting HP: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("[PlayerHP] TakeDamage called with: " + damage + " | isDead: " + isDead + " | isInvincible: " + isInvincible);
        if (isDead) { Debug.Log("[PlayerHP] Blocked — already dead."); return; }
        if (isInvincible) { Debug.Log("[PlayerHP] Blocked — invincible."); return; }

        currentHealth -= damage;
        Debug.Log("[PlayerHP] Player took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityRoutine());
    }

    public void AddMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float duration = baseInvincibilityTime;
        if (UpgradeState.Instance != null)
            duration += UpgradeState.Instance.invincibilityBonus;

        Debug.Log("[PlayerHP] Invincibility started for " + duration + "s");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("[PlayerHP] Invincibility ended.");
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("[PlayerHP] Player died. Resetting run.");

        TopDownController controller = GetComponent<TopDownController>();
        if (controller != null) controller.enabled = false;

        if (Zone1Manager.Instance != null)
            Zone1Manager.Instance.resetRun();
        else
            Debug.LogWarning("[PlayerHP] Could not reset run — Zone1Manager.Instance is null.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & projectileLayer) == 0) return;
        EnemyProjectileBase projectile = other.GetComponent<EnemyProjectileBase>();
        if (projectile == null) return;

        // check if block absorbs this projectile
        Weapon weapon = GetComponentInChildren<Weapon>();
        if (weapon != null && weapon.IsBlocking && !weapon.IsStunned)
        {
            if (IsProjectileInFront(other.transform))
            {
                // heavy hits drain the block meter rather than dealing damage
                if (projectile.Damage >= heavyHitThreshold)
                    weapon.DrainBlockMeter(projectile.Damage * blockMeterDrainPerDamage);

                projectile.TryClear();
                return;
            }
            // projectile came from behind — block doesn't apply
        }

        TakeDamage(projectile.Damage);
        projectile.TryClear();
    }

    // true if the projectile is within the player's forward hemisphere
    private bool IsProjectileInFront(Transform projectileTransform)
    {
        Vector3 toProjectile = projectileTransform.position - transform.position;
        toProjectile.y = 0f;
        Vector3 forward = transform.forward;
        forward.y = 0f;
        return Vector3.Dot(forward.normalized, toProjectile.normalized) > 0f;
    }
}