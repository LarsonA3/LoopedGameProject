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

    private Weapon weapon;
    [SerializeField] private float heavyHitThreshold = 5f;
    [SerializeField] private float blockMeterDrainPerDamage = 0.03f;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();

        if (UpgradeState.Instance != null)
            maxHealth += UpgradeState.Instance.maxHPBonus;

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (isInvincible) return;

        if (weapon != null && weapon.IsBlocking && !weapon.IsStunned)
        {
            weapon.DrainBlockMeter(damage * blockMeterDrainPerDamage);
            Debug.Log("[PlayerHP] Damage blocked, meter drained by: " + damage * blockMeterDrainPerDamage);
            return;
        }

        currentHealth -= damage;
        Debug.Log("[PlayerHP] Player took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f) { Die(); return; }

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
        {
            Zone1Manager.Instance.resetRun();
            Zone1Manager.Instance.ResetAfterPlayerDeath();
        }
        else
            Debug.LogWarning("[PlayerHP] Could not reset run — Zone1Manager.Instance is null.");
    }




}