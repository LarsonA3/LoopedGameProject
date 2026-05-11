using System.Collections.Generic;
using UnityEngine;

public class PlayerEpicCardAbilityController : MonoBehaviour
{
    private PlayerHP playerHP;
    private Weapon weapon;
    private TopDownController controller;

    // Enhanced Interrogation Techniques
    private readonly Dictionary<EnemyHP, List<DamageOverTimeStack>> activeDots =
        new Dictionary<EnemyHP, List<DamageOverTimeStack>>();

    // Capacitor Bleed
    private float capacitorBleedNextReadyTime;

    // Impact Memory
    private int blockedHitStacks;

    // Thermal Runaway
    private int thermalRunawayStacks;

    // Graze Momentum Driver
    private int grazeMomentumStacks;
    private float grazeMomentumExpireTime;

    private class DamageOverTimeStack
    {
        public float damagePerSecond;
        public float endTime;
        public float tickTimer;
    }

    private void Awake()
    {
        playerHP = GetComponent<PlayerHP>();
        weapon = GetComponentInChildren<Weapon>();
        controller = GetComponent<TopDownController>();
    }

    private void Update()
    {
        UpdateSelfMaintenanceFunction();
        UpdateDamageOverTime();
        UpdateGrazeMomentumTimer();
    }

    private int GetStacks(AbilityUpgradeType ability)
    {
        if (CardAbilityState.Instance == null)
        {
            return 0;
        }

        return CardAbilityState.Instance.GetStacks(ability);
    }

    // Call this from Weapon.cs before enemy.TakeDamage().
    public float ModifyOutgoingWeaponDamage(float baseDamage, EnemyHP enemy, bool isLightAttack, bool isHeavyAttack)
    {
        float damage = baseDamage;

        damage *= GetRedlineGovernorMultiplier();
        damage *= GetImpactMemoryMultiplier(isHeavyAttack);
        damage *= GetThermalRunawayMultiplier();

        return damage;
    }

    // Call this from Weapon.cs after enemy.TakeDamage().
    public void OnWeaponHit(EnemyHP enemy, float finalDamage, bool isLightAttack, bool isHeavyAttack)
    {
        HandleEnhancedInterrogationTechniques(enemy, finalDamage);
        HandleThermalRunawayHit();
    }

    // Call this from EnemyHP.Die().
    public void OnEnemyKilled(EnemyHP enemy)
    {
        HandleBadSectorSpread(enemy);
        HandleScrapFeedbackLoop();
    }

    // Call this from PlayerHP.cs after taking damage.
    public void OnPlayerTookDamage(float damageTaken)
    {
        thermalRunawayStacks = 0;
        HandleCapacitorBleed(damageTaken);
    }

    // Call this from TopDownController.cs when dash starts.
    public void OnDash()
    {
        HandleDashShockwave();
    }

    // Call this from Weapon.cs when player blocks a hit/projectile.
    public void OnBlockHit()
    {
        HandleImpactMemoryBlockedHit();
    }

    // Call this from Weapon.cs when a parry succeeds.
    public void OnParrySuccess()
    {
        HandleParryCapacitor();
        HandleCounterpulse();
        HandleParryRebootShortcut();
    }

    // Call this from GrazeDetector.cs when a projectile is grazed.
    public void OnGraze()
    {
        HandleGrazeMomentumDriver();
    }

    // Call this from Weapon.cs when reflecting a projectile.
    public void ModifyParriedProjectile(GameObject reflectedProjectile)
    {
        HandleProjectileReformatting(reflectedProjectile);
    }

    // ------------------------------------------------------------
    // 1. Self-Maintenance Function
    // ------------------------------------------------------------

    private void UpdateSelfMaintenanceFunction()
    {
        int stacks = GetStacks(AbilityUpgradeType.SelfMaintenanceFunction);
        if (stacks <= 0) return;
        if (playerHP == null) return;

        if (playerHP.CurrentHP >= playerHP.MaxHP * 0.5f)
        {
            return;
        }

        float healRate = 1f + 0.5f * (stacks - 1);
        healRate = Mathf.Min(healRate, 5f);

        float halfHP = playerHP.MaxHP * 0.5f;
        float healAmount = healRate * Time.deltaTime;

        if (playerHP.CurrentHP + healAmount > halfHP)
        {
            healAmount = halfHP - playerHP.CurrentHP;
        }

        if (healAmount > 0f)
        {
            playerHP.Heal(healAmount);
        }
    }

    // ------------------------------------------------------------
    // 2. Enhanced Interrogation Techniques
    // ------------------------------------------------------------

    private void HandleEnhancedInterrogationTechniques(EnemyHP enemy, float attackDamage)
    {
        int stacks = GetStacks(AbilityUpgradeType.EnhancedInterrogationTechniques);
        if (stacks <= 0) return;
        if (enemy == null) return;

        float percentPerSecond = 0.02f + 0.01f * (stacks - 1);
        percentPerSecond = Mathf.Min(percentPerSecond, 0.10f);

        float damagePerSecond = attackDamage * percentPerSecond;

        if (!activeDots.ContainsKey(enemy))
        {
            activeDots[enemy] = new List<DamageOverTimeStack>();
        }

        List<DamageOverTimeStack> dotList = activeDots[enemy];

        if (dotList.Count >= 5)
        {
            dotList.RemoveAt(0);
        }

        dotList.Add(new DamageOverTimeStack
        {
            damagePerSecond = damagePerSecond,
            endTime = Time.time + 5f,
            tickTimer = 0f
        });
    }

    private void UpdateDamageOverTime()
    {
        if (activeDots.Count == 0)
        {
            return;
        }

        List<EnemyHP> enemiesToRemove = new List<EnemyHP>();

        foreach (KeyValuePair<EnemyHP, List<DamageOverTimeStack>> pair in activeDots)
        {
            EnemyHP enemy = pair.Key;

            if (enemy == null)
            {
                enemiesToRemove.Add(enemy);
                continue;
            }

            List<DamageOverTimeStack> dotList = pair.Value;

            for (int i = dotList.Count - 1; i >= 0; i--)
            {
                DamageOverTimeStack dot = dotList[i];

                if (Time.time > dot.endTime)
                {
                    dotList.RemoveAt(i);
                    continue;
                }

                dot.tickTimer += Time.deltaTime;

                if (dot.tickTimer >= 1f)
                {
                    dot.tickTimer -= 1f;
                    enemy.TakeDamage(dot.damagePerSecond, gameObject);
                }
            }

            if (dotList.Count == 0)
            {
                enemiesToRemove.Add(enemy);
            }
        }

        foreach (EnemyHP enemy in enemiesToRemove)
        {
            activeDots.Remove(enemy);
        }
    }

    // ------------------------------------------------------------
    // 3. Bad Sector Spread
    // ------------------------------------------------------------

    private void HandleBadSectorSpread(EnemyHP deadEnemy)
    {
        int stacks = GetStacks(AbilityUpgradeType.BadSectorSpread);
        if (stacks <= 0) return;
        if (deadEnemy == null) return;

        float percent = 0.10f + 0.05f * (stacks - 1);
        percent = Mathf.Min(percent, 0.55f);

        float damage = deadEnemy.MaxHP * percent;

        DamageEnemiesAround(deadEnemy.transform.position, 3f, damage, deadEnemy);
    }

    // ------------------------------------------------------------
    // 4. Redline Governor
    // ------------------------------------------------------------

    private float GetRedlineGovernorMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.RedlineGovernor);
        if (stacks <= 0) return 1f;
        if (playerHP == null) return 1f;

        if (playerHP.CurrentHP > playerHP.MaxHP * 0.25f)
        {
            return 1f;
        }

        float bonus = 0.20f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.65f);

        return 1f + bonus;
    }

    // ------------------------------------------------------------
    // 5. Capacitor Bleed
    // ------------------------------------------------------------

    private void HandleCapacitorBleed(float damageTaken)
    {
        int stacks = GetStacks(AbilityUpgradeType.CapacitorBleed);
        if (stacks <= 0) return;

        if (Time.time < capacitorBleedNextReadyTime)
        {
            return;
        }

        float percent = 0.10f + 0.05f * (stacks - 1);
        percent = Mathf.Min(percent, 0.55f);

        float cooldown = 4f - 0.25f * (stacks - 1);
        cooldown = Mathf.Max(cooldown, 1.75f);

        float pulseDamage = damageTaken * percent;

        DamageEnemiesAround(transform.position, 3f, pulseDamage);

        capacitorBleedNextReadyTime = Time.time + cooldown;
    }

    // ------------------------------------------------------------
    // 6. Scrap Feedback Loop
    // ------------------------------------------------------------

    private void HandleScrapFeedbackLoop()
    {
        int stacks = GetStacks(AbilityUpgradeType.ScrapFeedbackLoop);
        if (stacks <= 0) return;
        if (playerHP == null) return;

        float healing = 2f + 1f * (stacks - 1);
        healing = Mathf.Min(healing, 11f);

        playerHP.Heal(healing);
    }

    // ------------------------------------------------------------
    // 7. Dash Shockwave
    // ------------------------------------------------------------

    private void HandleDashShockwave()
    {
        int stacks = GetStacks(AbilityUpgradeType.DashShockwave);
        if (stacks <= 0) return;

        float damage = 5f + 3f * (stacks - 1);
        damage = Mathf.Min(damage, 32f);

        DamageEnemiesAround(transform.position, 2.5f, damage);
    }

    // ------------------------------------------------------------
    // 8. Parry Capacitor
    // ------------------------------------------------------------

    private void HandleParryCapacitor()
    {
        int stacks = GetStacks(AbilityUpgradeType.ParryCapacitor);
        if (stacks <= 0) return;
        if (weapon == null) return;

        float restored = 0.5f * stacks;
        restored = Mathf.Min(restored, 5f);

        weapon.RestoreBlockMeter(restored);
    }

    // ------------------------------------------------------------
    // 9. Impact Memory
    // ------------------------------------------------------------

    private void HandleImpactMemoryBlockedHit()
    {
        int stacks = GetStacks(AbilityUpgradeType.ImpactMemory);
        if (stacks <= 0) return;

        blockedHitStacks = Mathf.Min(blockedHitStacks + 1, 5);
    }

    private float GetImpactMemoryMultiplier(bool isHeavyAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.ImpactMemory);
        if (stacks <= 0) return 1f;
        if (!isHeavyAttack) return 1f;
        if (blockedHitStacks <= 0) return 1f;

        float bonusPerBlockedHit = 0.05f + 0.025f * (stacks - 1);
        bonusPerBlockedHit = Mathf.Min(bonusPerBlockedHit, 0.25f);

        float totalBonus = bonusPerBlockedHit * blockedHitStacks;

        blockedHitStacks = 0;

        return 1f + totalBonus;
    }

    // ------------------------------------------------------------
    // 10. Thermal Runaway
    // ------------------------------------------------------------

    private void HandleThermalRunawayHit()
    {
        int stacks = GetStacks(AbilityUpgradeType.ThermalRunaway);
        if (stacks <= 0) return;

        thermalRunawayStacks = Mathf.Min(thermalRunawayStacks + 1, 5);
    }

    private float GetThermalRunawayMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.ThermalRunaway);
        if (stacks <= 0) return 1f;
        if (thermalRunawayStacks <= 0) return 1f;

        float bonusPerHit = 0.02f + 0.01f * (stacks - 1);
        bonusPerHit = Mathf.Min(bonusPerHit, 0.12f);

        return 1f + bonusPerHit * thermalRunawayStacks;
    }

    // ------------------------------------------------------------
    // 11. Graze Momentum Driver
    // ------------------------------------------------------------

    private void HandleGrazeMomentumDriver()
    {
        int stacks = GetStacks(AbilityUpgradeType.GrazeMomentumDriver);
        if (stacks <= 0) return;
        if (controller == null) return;

        grazeMomentumStacks = Mathf.Min(grazeMomentumStacks + 1, 5);
        grazeMomentumExpireTime = Time.time + 3f;

        float speedPerStack = 0.03f + 0.01f * (stacks - 1);
        speedPerStack = Mathf.Min(speedPerStack, 0.12f);

        float multiplier = 1f + speedPerStack * grazeMomentumStacks;

        controller.SetTemporaryMoveSpeedMultiplier(multiplier, 3f);
    }

    private void UpdateGrazeMomentumTimer()
    {
        if (grazeMomentumStacks <= 0) return;

        if (Time.time > grazeMomentumExpireTime)
        {
            grazeMomentumStacks = 0;

            if (controller != null)
            {
                controller.SetTemporaryMoveSpeedMultiplier(1f, 0f);
            }
        }
    }

    // ------------------------------------------------------------
    // 12. Projectile Reformatting
    // ------------------------------------------------------------

    private void HandleProjectileReformatting(GameObject reflectedProjectile)
    {
        int stacks = GetStacks(AbilityUpgradeType.ProjectileReformatting);
        if (stacks <= 0) return;
        if (reflectedProjectile == null) return;

        float bonusDamage = 0.25f + 0.05f * (stacks - 1);
        bonusDamage = Mathf.Min(bonusDamage, 0.70f);

        ParriedProjectile projectile = reflectedProjectile.GetComponent<ParriedProjectile>();

        if (projectile != null)
        {
            projectile.targetNearestEnemy = true;
            projectile.damageMultiplier += bonusDamage;
        }
    }

    // ------------------------------------------------------------
    // 13. Counterpulse
    // ------------------------------------------------------------

    private void HandleCounterpulse()
    {
        int stacks = GetStacks(AbilityUpgradeType.Counterpulse);
        if (stacks <= 0) return;

        float damage = 10f + 5f * (stacks - 1);
        damage = Mathf.Min(damage, 55f);

        DamageEnemiesAround(transform.position, 3f, damage);
    }

    // ------------------------------------------------------------
    // 14. Parry Reboot Shortcut
    // ------------------------------------------------------------

    private void HandleParryRebootShortcut()
    {
        int stacks = GetStacks(AbilityUpgradeType.ParryRebootShortcut);
        if (stacks <= 0) return;
        if (weapon == null) return;

        weapon.ClearBlockCooldown();

        float restored = 0.25f * stacks;
        restored = Mathf.Min(restored, 2.5f);

        weapon.RestoreBlockMeter(restored);
    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private void DamageEnemiesAround(Vector3 center, float radius, float damage, EnemyHP ignoredEnemy = null)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
            {
                continue;
            }

            EnemyHP enemy = hit.GetComponentInParent<EnemyHP>();

            if (enemy == null)
            {
                continue;
            }

            if (enemy == ignoredEnemy)
            {
                continue;
            }

            enemy.TakeDamage(damage, gameObject);
        }
    }
    public void ResetRunState()
    {
        Debug.Log("[EpicCards] Run state reset.");
    }

}
