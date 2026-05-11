using System;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Invincibility")]
    public float invincibilityTime = 0.5f;

    [Header("Blocking")]
    [SerializeField] private float blockMeterDrainPerDamage = 0.03f;

    private bool normalInvincible;
    private bool cardInvincible;
    private bool isDead;
    private float invincibilityTimer;

    private TopDownController controller;
    private Weapon weapon;

    public float CurrentHP => currentHealth;
    public float MaxHP => maxHealth;
    public bool IsDead => isDead;

    public event Action<float, float> OnHealthChanged;

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        weapon = GetComponentInChildren<Weapon>();

        if (UpgradeState.Instance != null)
        {
            maxHealth += UpgradeState.Instance.maxHPBonus;
            invincibilityTime += UpgradeState.Instance.invincibilityBonus;
        }

        currentHealth = Mathf.Clamp(maxHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0f)
            {
                normalInvincible = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("[PlayerHP] TakeDamage called for damage: " + damage + ". Current HP: " + currentHealth + "/" + maxHealth);
        if (isDead) return;
        if (damage <= 0f) return;
        if (normalInvincible || cardInvincible) return;

        PlayerRareCardAbilityController rareCards =
            GetComponent<PlayerRareCardAbilityController>();

        PlayerEpicCardAbilityController epicCards =
            GetComponent<PlayerEpicCardAbilityController>();

        PlayerLegendaryCardAbilityController legendaryCards =
            GetComponent<PlayerLegendaryCardAbilityController>();

        if (weapon != null && weapon.IsBlocking && !weapon.IsStunned)
        {
            weapon.DrainBlockMeter(damage * blockMeterDrainPerDamage);
            weapon.OnBlockedHit(damage);

            Debug.Log("[PlayerHP] Damage blocked. Block meter drained by: " + damage * blockMeterDrainPerDamage);

            return;
        }

        if (rareCards != null)
        {
            damage = rareCards.ModifyIncomingDamage(damage);
        }

        if (legendaryCards != null)
        {
            damage = legendaryCards.ModifyIncomingDamage(damage);
        }

        if (currentHealth - damage <= 0f)
        {
            if (legendaryCards != null)
            {
                bool survived = legendaryCards.TryPreventLethalDamage(ref damage);

                if (survived)
                {
                    OnHealthChanged?.Invoke(currentHealth, maxHealth);
                    StartInvincibility();
                    return;
                }
            }
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("[PlayerHP] Current HP After Hit: " + currentHealth + "/" + maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (rareCards != null)
        {
            rareCards.OnPlayerTookDamage(damage);
        }

        if (epicCards != null)
        {
            epicCards.OnPlayerTookDamage(damage);
        }

        if (legendaryCards != null)
        {
            legendaryCards.OnPlayerTookDamage(damage);
        }

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        StartInvincibility();
    }

    private void StartInvincibility()
    {
        normalInvincible = true;
        invincibilityTimer = invincibilityTime;
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddMaxHealth(float amount)
    {
        if (amount <= 0f) return;

        maxHealth += amount;
        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddInvincibilityFrames(float amount)
    {
        if (amount <= 0f) return;

        invincibilityTime += amount;
    }

    public void SetHealth(float amount)
    {
        currentHealth = Mathf.Clamp(amount, 0f, maxHealth);

        if (currentHealth > 0f)
        {
            isDead = false;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetInvincible(bool value)
    {
        cardInvincible = value;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0f;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("[PlayerHP] Player died.");

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (weapon != null)
        {
            weapon.enabled = false;
        }

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        DeathManager deathManager = FindObjectOfType<DeathManager>();

        if (deathManager != null)
        {
            deathManager.HandlePlayerDeath(this);
        }
        else
        {
            Debug.LogWarning("[PlayerHP] No DeathManager found. Player is dead but no reset behavior exists.");
        }
    }

    public void ReviveAt(Vector3 position, float healthAmount)
    {
        isDead = false;
        currentHealth = Mathf.Clamp(healthAmount, 1f, maxHealth);

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = position;
            characterController.enabled = true;
        }
        else
        {
            transform.position = position;
        }

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (weapon != null)
        {
            weapon.enabled = true;
        }

        StartInvincibility();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("[PlayerHP] Player revived.");
    }
}
