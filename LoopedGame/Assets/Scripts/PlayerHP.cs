using System.Collections;
using UnityEngine;
public class PlayerHP : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [Header("Damage")]
    [SerializeField] private float baseInvincibilityTime = 0.3f;
    private bool isDead = false;
    private bool isInvincible = false;
    private PlayerWeaponAttack weaponAttack;

    [Header("Layers")]
    [SerializeField] private LayerMask projectileLayer;

    private void Awake()
    {
        weaponAttack = GetComponent<PlayerWeaponAttack>();
        Debug.Log("[PlayerHP] Awake. weaponAttack found: " + (weaponAttack != null));
        if (UpgradeState.Instance != null)
        {
            maxHealth += UpgradeState.Instance.maxHPBonus;
        }
        currentHealth = maxHealth;
        Debug.Log("[PlayerHP] Starting HP: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("[PlayerHP] Player took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);
        Debug.Log("[PlayerHP] TakeDamage called with: " + damage + " | isDead: " + isDead + " | isInvincible: " + isInvincible);
        if (isDead) { Debug.Log("[PlayerHP] Blocked — already dead."); return; }
        if (isInvincible) { Debug.Log("[PlayerHP] Blocked — invincible."); return; }

        KineticRiotShieldWeapon shield = GetCurrentShield();
        if (shield != null && shield.IsBlocking)
        {
            shield.AbsorbHit(damage);
            Debug.Log("[PlayerHP] Shield absorbed " + damage + " damage.");
            return;
        }

        currentHealth -= damage;
        Debug.Log("[PlayerHP] Player took " + damage + " damage. HP: " + currentHealth);

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
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private KineticRiotShieldWeapon GetCurrentShield()
    {
        if (weaponAttack == null) return null;
        if (weaponAttack.CurrentWeapon == null) return null;
        return weaponAttack.CurrentWeapon as KineticRiotShieldWeapon;
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float duration = baseInvincibilityTime;
        if (UpgradeState.Instance != null)
        {
            duration += UpgradeState.Instance.invincibilityBonus;
        }
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

        PlayerWeaponAttack attack = GetComponent<PlayerWeaponAttack>();
        if (attack != null) attack.enabled = false;

        if (Zone1Manager.Instance != null)
        {
            Zone1Manager.Instance.resetRun();
        }
        else
        {
            Debug.LogWarning("[PlayerHP] Could not reset run — Zone1Manager.Instance is null.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[PlayerHP] OnTriggerEnter fired with: " + other.gameObject.name + " on layer: " + LayerMask.LayerToName(other.gameObject.layer));
        Debug.Log("[PlayerHP] ProjectileLayer value: " + projectileLayer.value + " | Object layer bit: " + (1 << other.gameObject.layer));

        if (((1 << other.gameObject.layer) & projectileLayer) == 0)
        {
            Debug.Log("[PlayerHP] Layer mismatch — not a projectile layer.");
            return;
        }

        EnemyProjectileBase projectile = other.GetComponent<EnemyProjectileBase>();
        Debug.Log("[PlayerHP] EnemyProjectileBase found: " + (projectile != null));
        if (projectile == null) return;

        TakeDamage(projectile.Damage);
        projectile.TryClear();
    }
}