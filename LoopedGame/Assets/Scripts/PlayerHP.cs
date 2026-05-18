using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Invincibility")]
    public float invincibilityTime = 0.5f;

    [Header("Blocking")]
    [SerializeField] private float blockMeterDrainPerDamage = 0.03f;

    private bool normalInvincible;
    private bool cardInvincible;
    private bool isDead;
    private float invincibilityTimer;
    private GameObject hitParticlePrefab;

    private TopDownController controller;
    private Weapon weapon;

    private CameraMoveWithPlayer cam;

    public float CurrentHP => currentHealth;
    public float MaxHP => maxHealth;
    public bool IsDead => isDead;

    public event Action<float, float> OnHealthChanged;

    private void Awake()
    {
        controller = GetComponent<TopDownController>();
        weapon = GetComponentInChildren<Weapon>();
        cam = FindObjectOfType<CameraMoveWithPlayer>();

        if (UpgradeState.Instance != null)
        {
            maxHealth += UpgradeState.Instance.maxHPBonus;
            invincibilityTime += UpgradeState.Instance.invincibilityBonus;
        }

        maxHealth *= DifficultySettings.Selected switch
        {
            Difficulty.Easy => 1.0f,
            Difficulty.Medium => 0.7f,
            Difficulty.Hard => 0.4f,
            Difficulty.Nightmare => 0.25f,
            _ => 1.0f
        };

        currentHealth = maxHealth;
        isDead = false;

        OnHealthChanged += UpdateHealthSlider;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        hitParticlePrefab = Resources.Load<GameObject>("Particles/hitParticlePLAYER");
        if (hitParticlePrefab == null)
            Debug.LogWarning("[PlayerHP] Could not find Resources/Particles/hitParticlePLAYER");
    }

    private void UpdateHealthSlider(float current, float max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    private void Update()
    {
        UpdateInvincibility();
    }

    private void UpdateInvincibility()
    {
        if (invincibilityTimer <= 0f)
        {
            return;
        }

        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0f)
        {
            normalInvincible = false;
            invincibilityTimer = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0f)
        {
            return;
        }

        if (normalInvincible || cardInvincible)
        {
            return;
        }

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

            Debug.Log("[PlayerHP] Damage blocked. Incoming damage: " + damage);

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

        if (currentHealth - damage <= 0f && legendaryCards != null)
        {
            bool survived = legendaryCards.TryPreventLethalDamage(ref damage);

            if (survived)
            {
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                StartInvincibility();
                return;
            }
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (hitParticlePrefab != null)
        {
            GameObject fx = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(fx, ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 2f);
        }
        SoundManager.PlaySound("Playerhit");
        if (cam != null) cam.TriggerShake();


        Debug.Log("[PlayerHP] Player took " + damage + " damage. HP: " + currentHealth + "/" + maxHealth);

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
        if (isDead)
        {
            return;
        }

        if (amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log("[PlayerHP] Player healed for " + amount + ". HP: " + currentHealth + "/" + maxHealth);
    }

    public void AddMaxHealth(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        maxHealth += amount;
        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddInvincibilityFrames(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

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

    public void ClearAllInvincibility()
    {
        normalInvincible = false;
        cardInvincible = false;
        invincibilityTimer = 0f;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        FindObjectOfType<HScore>().FinalScore(); // CALL FINAL SCORE
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

        if (Zone1Manager.Instance != null)
        {
            Zone1Manager.Instance.ResetAfterPlayerDeath();
        }

        else
        {
            Debug.LogWarning("[PlayerHP] Zone1Manager instance not found. Cannot reset run on death.");
        }
    }

    public void ReviveAt(Vector3 position, float healthAmount)
    {
        isDead = false;
        currentHealth = Mathf.Clamp(healthAmount, 1f, maxHealth);

        ClearAllInvincibility();

        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = position;
            Physics.SyncTransforms();
            characterController.enabled = true;
        }
        else
        {
            transform.position = position;
            Physics.SyncTransforms();
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

        Debug.Log("[PlayerHP] Player revived at " + position + " with HP: " + currentHealth);
    }
}