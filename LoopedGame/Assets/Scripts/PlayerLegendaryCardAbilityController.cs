using UnityEngine;

public enum CombatActionType
{
    None,
    LightAttack,
    HeavyAttack,
    Block,
    Parry,
    Dash,
    Graze
}

public class PlayerLegendaryCardAbilityController : MonoBehaviour
{
    private PlayerHP playerHP;
    private Weapon weapon;
    public CardRarity cardRarity;
    private TopDownController controller;

    // Emergency Repairs
    private bool emergencyRepairsUsedThisRun;

    // Phase Shift Matrix
    private float phaseShiftNextReadyTime;
    private bool phaseShiftActive;
    private float phaseShiftEndTime;

    // Weaponized Telemetry
    private int lightAttackCount;
    private int heavyAttackCount;
    private int blockCount;
    private int parryCount;
    private int dashCount;
    private int grazeCount;

    private CombatActionType nextRoomEmpoweredAction = CombatActionType.None;
    private float nextRoomEmpowerBonus;
    private bool nextRoomEmpowerReady;

    // Exponential Threat Model
    private float exponentialThreatDamageBonus;

    // Perfect Counterchain
    private bool perfectCounterchainReady;
    private float perfectCounterchainEndTime;
    private float perfectCounterchainPulseDamage;

    private void Awake()
    {
        playerHP = GetComponent<PlayerHP>();
        weapon = GetComponentInChildren<Weapon>();
        controller = GetComponent<TopDownController>();
    }

    private void Update()
    {
        UpdatePhaseShiftTimer();
    }

    private int GetStacks(AbilityUpgradeType ability)
    {
        if (CardAbilityState.Instance == null)
        {
            return 0;
        }

        return CardAbilityState.Instance.GetStacks(ability);
    }

    // Call from Weapon.cs before enemy.TakeDamage().
    public float ModifyOutgoingWeaponDamage(float baseDamage, EnemyHP enemy, bool isLightAttack, bool isHeavyAttack)
    {
        float damage = baseDamage;

        damage *= GetWeaponizedTelemetryDamageMultiplier(isLightAttack, isHeavyAttack);
        damage *= GetExponentialThreatModelMultiplier();
        damage *= GetCriticalSystemAuthorityDamageMultiplier();
        damage *= GetPerfectCounterchainMultiplier();

        return damage;
    }

    // Call from PlayerHP.cs before subtracting health.
    public float ModifyIncomingDamage(float incomingDamage)
    {
        incomingDamage = ApplyCriticalSystemAuthorityDefense(incomingDamage);

        return incomingDamage;
    }

    // Call from PlayerHP.cs before final death is accepted.
    // Return true if a Legendary card saved the player.
    public bool TryPreventLethalDamage(ref float damage)
    {
        if (TrySyntheticSurvivalInstinct(ref damage))
        {
            return damage <= 0f;
        }

        if (TryEmergencyRepairs())
        {
            damage = 0f;
            return true;
        }

        return false;
    }

    // Call from PlayerHP.cs after damage is actually taken.
    public void OnPlayerTookDamage(float damageTaken)
    {
        HandlePhaseShiftMatrix();
    }

    // Call from Weapon.cs after successful weapon hit.
    public void OnWeaponHit(EnemyHP enemy, bool isLightAttack, bool isHeavyAttack)
    {
        if (isLightAttack)
        {
            RegisterCombatAction(CombatActionType.LightAttack);
        }

        if (isHeavyAttack)
        {
            RegisterCombatAction(CombatActionType.HeavyAttack);
        }

        if (perfectCounterchainPulseDamage > 0f && enemy != null)
        {
            DamageEnemiesAround(enemy.transform.position, 2.5f, perfectCounterchainPulseDamage, enemy);
            perfectCounterchainPulseDamage = 0f;
        }
    }

    // Call from EnemyHP.Die().
    public void OnEnemyKilled(EnemyHP enemy)
    {
        HandleExponentialThreatModel();
    }

    // Call from TopDownController.cs when dash starts.
    public void OnDash()
    {
        RegisterCombatAction(CombatActionType.Dash);
    }

    // Call from Weapon.cs when player blocks a hit/projectile.
    public void OnBlockHit()
    {
        RegisterCombatAction(CombatActionType.Block);
    }

    // Call from Weapon.cs when parry succeeds.
    public void OnParrySuccess()
    {
        RegisterCombatAction(CombatActionType.Parry);
        HandlePerfectCounterchainParry();
    }

    // Call from GrazeDetector.cs when projectile is grazed.
    public void OnGraze()
    {
        RegisterCombatAction(CombatActionType.Graze);
    }

    // Call from RoomHandler.cs once when room clears.
    public void OnRoomCleared()
    {
        HandleWeaponizedTelemetryRoomClear();

        // Exponential Threat Model lasts only for the current room.
        exponentialThreatDamageBonus = 0f;

        // Reset per-room action counts.
        lightAttackCount = 0;
        heavyAttackCount = 0;
        blockCount = 0;
        parryCount = 0;
        dashCount = 0;
        grazeCount = 0;
    }

    // Call when a new run starts if "once per run" should refresh.
    public void ResetRunLegendaryFlags()
    {
        emergencyRepairsUsedThisRun = false;
    }

    // ------------------------------------------------------------
    // 1. Emergency Repairs
    // ------------------------------------------------------------

    private bool TryEmergencyRepairs()
    {
        int stacks = GetStacks(AbilityUpgradeType.EmergencyRepairs);
        if (stacks <= 0) return false;
        if (playerHP == null) return false;
        if (emergencyRepairsUsedThisRun) return false;

        emergencyRepairsUsedThisRun = true;

        float recoveryHP = 10f + 10f * (stacks - 1);
        recoveryHP = Mathf.Min(recoveryHP, 100f);

        playerHP.SetHealth(recoveryHP);

        Debug.Log("[Emergency Repairs] Prevented death. Recovered to " + recoveryHP + " HP.");

        return true;
    }

    // ------------------------------------------------------------
    // 2. Phase Shift Matrix
    // ------------------------------------------------------------

    private void HandlePhaseShiftMatrix()
    {
        int stacks = GetStacks(AbilityUpgradeType.PhaseShiftMatrix);
        if (stacks <= 0) return;
        if (playerHP == null) return;
        if (Time.time < phaseShiftNextReadyTime) return;
        if (phaseShiftActive) return;

        if (playerHP.CurrentHP > playerHP.MaxHP * 0.10f)
        {
            return;
        }

        float duration = 1f + 0.25f * (stacks - 1);
        duration = Mathf.Min(duration, 3.5f);

        phaseShiftActive = true;
        phaseShiftEndTime = Time.time + duration;
        phaseShiftNextReadyTime = Time.time + 30f;

        playerHP.SetInvincible(true);

        Debug.Log("[Phase Shift Matrix] Active for " + duration + " seconds.");
    }

    private void UpdatePhaseShiftTimer()
    {
        if (!phaseShiftActive) return;

        if (Time.time >= phaseShiftEndTime)
        {
            phaseShiftActive = false;

            if (playerHP != null)
            {
                playerHP.SetInvincible(false);
            }

            Debug.Log("[Phase Shift Matrix] Ended.");
        }
    }

    // ------------------------------------------------------------
    // 3. Weaponized Telemetry
    // ------------------------------------------------------------

    private void RegisterCombatAction(CombatActionType action)
    {
        switch (action)
        {
            case CombatActionType.LightAttack:
                lightAttackCount++;
                break;

            case CombatActionType.HeavyAttack:
                heavyAttackCount++;
                break;

            case CombatActionType.Block:
                blockCount++;
                break;

            case CombatActionType.Parry:
                parryCount++;
                break;

            case CombatActionType.Dash:
                dashCount++;
                break;

            case CombatActionType.Graze:
                grazeCount++;
                break;
        }
    }

    private void HandleWeaponizedTelemetryRoomClear()
    {
        int stacks = GetStacks(AbilityUpgradeType.WeaponizedTelemetry);
        if (stacks <= 0) return;

        CombatActionType mostUsed = GetMostUsedCombatAction();

        if (mostUsed == CombatActionType.None)
        {
            return;
        }

        float bonus = 0.20f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.65f);

        nextRoomEmpoweredAction = mostUsed;
        nextRoomEmpowerBonus = bonus;
        nextRoomEmpowerReady = true;

        Debug.Log("[Weaponized Telemetry] Empowering " + mostUsed + " next room by " + bonus);
    }

    private CombatActionType GetMostUsedCombatAction()
    {
        int bestCount = 0;
        CombatActionType bestAction = CombatActionType.None;

        CheckBest(CombatActionType.LightAttack, lightAttackCount, ref bestAction, ref bestCount);
        CheckBest(CombatActionType.HeavyAttack, heavyAttackCount, ref bestAction, ref bestCount);
        CheckBest(CombatActionType.Block, blockCount, ref bestAction, ref bestCount);
        CheckBest(CombatActionType.Parry, parryCount, ref bestAction, ref bestCount);
        CheckBest(CombatActionType.Dash, dashCount, ref bestAction, ref bestCount);
        CheckBest(CombatActionType.Graze, grazeCount, ref bestAction, ref bestCount);

        return bestAction;
    }

    private void CheckBest(CombatActionType action, int count, ref CombatActionType bestAction, ref int bestCount)
    {
        if (count > bestCount)
        {
            bestCount = count;
            bestAction = action;
        }
    }

    private float GetWeaponizedTelemetryDamageMultiplier(bool isLightAttack, bool isHeavyAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.WeaponizedTelemetry);
        if (stacks <= 0) return 1f;
        if (!nextRoomEmpowerReady) return 1f;

        bool matches =
            (nextRoomEmpoweredAction == CombatActionType.LightAttack && isLightAttack) ||
            (nextRoomEmpoweredAction == CombatActionType.HeavyAttack && isHeavyAttack);

        if (!matches)
        {
            return 1f;
        }

        return 1f + nextRoomEmpowerBonus;
    }

    public float GetWeaponizedTelemetryDashMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.WeaponizedTelemetry);
        if (stacks <= 0) return 1f;
        if (!nextRoomEmpowerReady) return 1f;

        if (nextRoomEmpoweredAction != CombatActionType.Dash)
        {
            return 1f;
        }

        return 1f + nextRoomEmpowerBonus;
    }

    public float GetWeaponizedTelemetryBlockMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.WeaponizedTelemetry);
        if (stacks <= 0) return 1f;
        if (!nextRoomEmpowerReady) return 1f;

        if (nextRoomEmpoweredAction != CombatActionType.Block)
        {
            return 1f;
        }

        return 1f + nextRoomEmpowerBonus;
    }

    public float GetWeaponizedTelemetryParryMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.WeaponizedTelemetry);
        if (stacks <= 0) return 1f;
        if (!nextRoomEmpowerReady) return 1f;

        if (nextRoomEmpoweredAction != CombatActionType.Parry)
        {
            return 1f;
        }

        return 1f + nextRoomEmpowerBonus;
    }

    // ------------------------------------------------------------
    // 4. Exponential Threat Model
    // ------------------------------------------------------------

    private void HandleExponentialThreatModel()
    {
        int stacks = GetStacks(AbilityUpgradeType.ExponentialThreatModel);
        if (stacks <= 0) return;

        float bonusPerKill = 0.02f + 0.01f * (stacks - 1);
        bonusPerKill = Mathf.Min(bonusPerKill, 0.11f);

        exponentialThreatDamageBonus += bonusPerKill;
    }

    private float GetExponentialThreatModelMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.ExponentialThreatModel);
        if (stacks <= 0) return 1f;

        return 1f + exponentialThreatDamageBonus;
    }

    // ------------------------------------------------------------
    // 5. Critical System Authority
    // ------------------------------------------------------------

    private float GetCriticalSystemAuthorityDamageMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.CriticalSystemAuthority);
        if (stacks <= 0) return 1f;
        if (playerHP == null) return 1f;

        if (playerHP.CurrentHP > playerHP.MaxHP * 0.25f)
        {
            return 1f;
        }

        float bonus = 0.15f + 0.03f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.42f);

        return 1f + bonus;
    }

    private float ApplyCriticalSystemAuthorityDefense(float damage)
    {
        int stacks = GetStacks(AbilityUpgradeType.CriticalSystemAuthority);
        if (stacks <= 0) return damage;
        if (playerHP == null) return damage;

        if (playerHP.CurrentHP > playerHP.MaxHP * 0.25f)
        {
            return damage;
        }

        float reduction = 0.15f + 0.03f * (stacks - 1);
        reduction = Mathf.Min(reduction, 0.42f);

        return damage * (1f - reduction);
    }

    // ------------------------------------------------------------
    // 6. Perfect Counterchain
    // ------------------------------------------------------------

    private void HandlePerfectCounterchainParry()
    {
        int stacks = GetStacks(AbilityUpgradeType.PerfectCounterchain);
        if (stacks <= 0) return;
        if (weapon == null) return;

        perfectCounterchainReady = true;
        perfectCounterchainEndTime = Time.time + 2f;

        float pulseDamage = weapon.damageAmount * (1.5f + 0.5f * (stacks - 1));
        pulseDamage = Mathf.Min(pulseDamage, weapon.damageAmount * 4f);

        perfectCounterchainPulseDamage = pulseDamage;
    }

    private float GetPerfectCounterchainMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.PerfectCounterchain);
        if (stacks <= 0) return 1f;
        if (!perfectCounterchainReady) return 1f;

        if (Time.time > perfectCounterchainEndTime)
        {
            perfectCounterchainReady = false;
            perfectCounterchainPulseDamage = 0f;
            return 1f;
        }

        perfectCounterchainReady = false;

        float bonus = 0.30f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.75f);

        return 1f + bonus;
    }

    // ------------------------------------------------------------
    // 7. Synthetic Survival Instinct
    // ------------------------------------------------------------

    private bool TrySyntheticSurvivalInstinct(ref float damage)
    {
        int stacks = GetStacks(AbilityUpgradeType.SyntheticSurvivalInstinct);
        if (stacks <= 0) return false;
        if (weapon == null) return false;
        if (playerHP == null) return false;

        float projectedHP = playerHP.CurrentHP - damage;

        if (projectedHP > 0f)
        {
            return false;
        }

        float damagePreventedPerMeter = 10f + 5f * (stacks - 1);
        damagePreventedPerMeter = Mathf.Min(damagePreventedPerMeter, 55f);

        float availableBlockMeter = weapon.GetCurrentBlockMeter();

        if (availableBlockMeter <= 0f)
        {
            return false;
        }

        float damagePreventable = availableBlockMeter * damagePreventedPerMeter;

        float damageToPrevent = Mathf.Min(damage, damagePreventable);
        float meterNeeded = damageToPrevent / damagePreventedPerMeter;

        weapon.ConsumeBlockMeter(meterNeeded);

        damage -= damageToPrevent;

        Debug.Log("[Synthetic Survival Instinct] Prevented " + damageToPrevent + " damage using " + meterNeeded + " block meter.");

        return true;
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


}
