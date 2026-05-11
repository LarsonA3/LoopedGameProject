using System.Collections.Generic;
using UnityEngine;

public class PlayerRareCardAbilityController : MonoBehaviour
{
    private PlayerHP playerHP;
    private Weapon weapon;
    private TopDownController controller;

    private float lastDamageTakenTime = -999f;

    private int terminationKillCount;
    private float terminationKillWindowEndTime;
    private float terminationBuffEndTime;

    private int grazeCapacitorCharges;

    private float storedImpactBonusDamage;

    private float retaliationEndTime;

    private bool reactiveArmorReady;
    private float reactiveArmorReduction;

    private bool armorChecksumReady;

    private EnemyHP recursiveLastEnemy;
    private int recursiveHitCount;

    private int weaponHitCounter;

    private readonly Dictionary<EnemyHP, float> contactMarks = new Dictionary<EnemyHP, float>();

    private void Awake()
    {
        playerHP = GetComponent<PlayerHP>();
        weapon = GetComponentInChildren<Weapon>();
        controller = GetComponent<TopDownController>();
    }

    private void Update()
    {
        UpdateArmorChecksum();
        UpdateShieldedDiagnostics();
        UpdateKineticBrake();
        CleanupContactMarks();
    }

    private int GetStacks(AbilityUpgradeType ability)
    {
        if (CardAbilityState.Instance == null)
        {
            return 0;
        }

        return CardAbilityState.Instance.GetStacks(ability);
    }

    public float ModifyOutgoingWeaponDamage(float baseDamage, EnemyHP enemy, bool isLightAttack, bool isHeavyAttack)
    {
        float damage = baseDamage;

        damage *= GetTerminationProtocolMultiplier();
        damage *= GetGrazeCapacitorMultiplier(isLightAttack);
        damage *= GetWeakSignalAmplifierMultiplier(enemy);
        damage += ConsumeImpactMemoryBufferBonus();
        damage *= GetRetaliationSubroutineMultiplier();
        damage *= GetMomentumCacheMultiplier();
        damage *= GetRecursiveStrikeLogicMultiplier(enemy);
        damage *= GetContactDebuggerMultiplier(enemy, isHeavyAttack);

        return damage;
    }

    public float ModifyIncomingDamage(float damage)
    {
        damage = ApplyArmorChecksum(damage);
        damage = ApplyReactiveArmorPlating(damage);
        damage = ApplyEvasiveRecalculation(damage);

        return damage;
    }

    public void OnPlayerTookDamage(float damageTaken)
    {
        lastDamageTakenTime = Time.time;
        armorChecksumReady = false;

        HandleImpactMemoryBuffer(damageTaken);
        HandleRetaliationSubroutine();
        HandleReactiveArmorPlating();
    }

    public void OnWeaponHit(EnemyHP enemy, bool isLightAttack, bool isHeavyAttack)
    {
        weaponHitCounter++;

        HandleFailurePointPrioritizer(enemy);
        HandleOverheatVent();
        HandleContactDebugger(enemy, isLightAttack);
        HandleUnstableMotorTiming(isLightAttack);
    }

    public void OnEnemyKilled(EnemyHP enemy)
    {
        HandleTerminationProtocol();
    }

    public void OnRoomCleared()
    {
        HandleCombatCacheFlush();
    }

    public void OnDash()
    {
        // Evasive Recalculation and Momentum Cache use controller.LastDashTime.
        // Collision Override is handled through OnDashHitEnemy.
    }

    public void OnGraze(GameObject projectile)
    {
        HandleGrazeCapacitor();
    }

    public void OnDashHitEnemy(EnemyHP enemy)
    {
        HandleCollisionOverride(enemy);
    }

    private void HandleTerminationProtocol()
    {
        int stacks = GetStacks(AbilityUpgradeType.TerminationProtocol);
        if (stacks <= 0) return;

        if (Time.time > terminationKillWindowEndTime)
        {
            terminationKillCount = 0;
        }

        terminationKillCount++;
        terminationKillWindowEndTime = Time.time + 5f;

        if (terminationKillCount >= 2)
        {
            float duration = 5f + 0.5f * (stacks - 1);
            duration = Mathf.Min(duration, 10f);

            terminationBuffEndTime = Time.time + duration;
        }
    }

    private float GetTerminationProtocolMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.TerminationProtocol);
        if (stacks <= 0) return 1f;
        if (Time.time > terminationBuffEndTime) return 1f;

        float bonus = 0.20f + 0.01f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.30f);

        return 1f + bonus;
    }

    private void HandleGrazeCapacitor()
    {
        int stacks = GetStacks(AbilityUpgradeType.GrazeCapacitor);
        if (stacks <= 0) return;

        grazeCapacitorCharges++;
    }

    private float GetGrazeCapacitorMultiplier(bool isLightAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.GrazeCapacitor);
        if (stacks <= 0) return 1f;
        if (!isLightAttack) return 1f;
        if (grazeCapacitorCharges < 5) return 1f;

        grazeCapacitorCharges = 0;

        float bonus = 0.05f * stacks;
        bonus = Mathf.Min(bonus, 0.50f);

        return 1f + bonus;
    }

    private void HandleFailurePointPrioritizer(EnemyHP enemy)
    {
        int stacks = GetStacks(AbilityUpgradeType.FailurePointPrioritizer);
        if (stacks <= 0) return;
        if (enemy == null) return;
        if (enemy.IsBoss) return;

        float chance = 0.025f * stacks;
        chance = Mathf.Min(chance, 0.25f);

        if (Random.value > chance) return;

        float duration = 2.3f + 0.3f * (stacks - 1);
        duration = Mathf.Min(duration, 5f);

        enemy.StunFor(duration);
    }

    private float GetWeakSignalAmplifierMultiplier(EnemyHP enemy)
    {
        int stacks = GetStacks(AbilityUpgradeType.WeakSignalAmplifier);
        if (stacks <= 0) return 1f;
        if (enemy == null) return 1f;

        if (enemy.CurrentHP > enemy.MaxHP * 0.25f)
        {
            return 1f;
        }

        float bonus = 0.15f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.60f);

        return 1f + bonus;
    }

    private void UpdateArmorChecksum()
    {
        int stacks = GetStacks(AbilityUpgradeType.ArmorChecksum);
        if (stacks <= 0) return;

        if (Time.time - lastDamageTakenTime >= 10f)
        {
            armorChecksumReady = true;
        }
    }

    private float ApplyArmorChecksum(float damage)
    {
        int stacks = GetStacks(AbilityUpgradeType.ArmorChecksum);
        if (stacks <= 0) return damage;
        if (!armorChecksumReady) return damage;

        armorChecksumReady = false;

        float reduction = 0.20f + 0.05f * (stacks - 1);
        reduction = Mathf.Min(reduction, 0.65f);

        return damage * (1f - reduction);
    }

    private void HandleCombatCacheFlush()
    {
        int stacks = GetStacks(AbilityUpgradeType.CombatCacheFlush);
        if (stacks <= 0) return;
        if (playerHP == null) return;

        float heal = 5f + 2.5f * (stacks - 1);
        heal = Mathf.Min(heal, 27.5f);

        playerHP.Heal(heal);
    }

    private void HandleImpactMemoryBuffer(float damageTaken)
    {
        int stacks = GetStacks(AbilityUpgradeType.ImpactMemoryBuffer);
        if (stacks <= 0) return;

        float percent = 0.20f + 0.05f * (stacks - 1);
        percent = Mathf.Min(percent, 0.65f);

        storedImpactBonusDamage = damageTaken * percent;
    }

    private float ConsumeImpactMemoryBufferBonus()
    {
        if (storedImpactBonusDamage <= 0f)
        {
            return 0f;
        }

        float bonus = storedImpactBonusDamage;
        storedImpactBonusDamage = 0f;

        return bonus;
    }

    private void HandleRetaliationSubroutine()
    {
        int stacks = GetStacks(AbilityUpgradeType.RetaliationSubroutine);
        if (stacks <= 0) return;

        float window = 3f + 0.5f * (stacks - 1);
        window = Mathf.Min(window, 7.5f);

        retaliationEndTime = Time.time + window;
    }

    private float GetRetaliationSubroutineMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.RetaliationSubroutine);
        if (stacks <= 0) return 1f;
        if (Time.time > retaliationEndTime) return 1f;

        retaliationEndTime = 0f;

        float bonus = 0.25f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.70f);

        return 1f + bonus;
    }

    private float ApplyEvasiveRecalculation(float damage)
    {
        int stacks = GetStacks(AbilityUpgradeType.EvasiveRecalculation);
        if (stacks <= 0) return damage;
        if (controller == null) return damage;

        float duration = 1f + 0.2f * (stacks - 1);
        duration = Mathf.Min(duration, 2.8f);

        if (Time.time - controller.LastDashTime > duration)
        {
            return damage;
        }

        float reduction = 0.10f + 0.03f * (stacks - 1);
        reduction = Mathf.Min(reduction, 0.37f);

        return damage * (1f - reduction);
    }

    private float GetMomentumCacheMultiplier()
    {
        int stacks = GetStacks(AbilityUpgradeType.MomentumCache);
        if (stacks <= 0) return 1f;
        if (controller == null) return 1f;

        if (Time.time - controller.LastDashTime > 1f)
        {
            return 1f;
        }

        float bonus = 0.20f + 0.05f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.65f);

        return 1f + bonus;
    }

    private void HandleCollisionOverride(EnemyHP enemy)
    {
        int stacks = GetStacks(AbilityUpgradeType.CollisionOverride);
        if (stacks <= 0) return;
        if (enemy == null) return;

        float damage = 5f + 5f * (stacks - 1);
        damage = Mathf.Min(damage, 50f);

        enemy.TakeDamage(damage, gameObject);
        enemy.KnockbackFrom(transform.position, 3f);
    }

    private float GetRecursiveStrikeLogicMultiplier(EnemyHP enemy)
    {
        int stacks = GetStacks(AbilityUpgradeType.RecursiveStrikeLogic);
        if (stacks <= 0) return 1f;
        if (enemy == null) return 1f;

        if (recursiveLastEnemy != enemy)
        {
            recursiveLastEnemy = enemy;
            recursiveHitCount = 1;
            return 1f;
        }

        recursiveHitCount++;

        if (recursiveHitCount < 3)
        {
            return 1f;
        }

        recursiveHitCount = 0;

        float bonus = 0.05f * stacks;
        bonus = Mathf.Min(bonus, 0.50f);

        return 1f + bonus;
    }

    private void HandleOverheatVent()
    {
        int stacks = GetStacks(AbilityUpgradeType.OverheatVent);
        if (stacks <= 0) return;

        if (weaponHitCounter % 5 != 0) return;

        float damage = 3f * stacks;
        damage = Mathf.Min(damage, 30f);

        DamageEnemiesAround(transform.position, 3f, damage);
    }

    private void UpdateKineticBrake()
    {
        int stacks = GetStacks(AbilityUpgradeType.KineticBrake);
        if (stacks <= 0) return;
        if (weapon == null) return;
        if (!weapon.IsBlocking) return;

        float slow = 0.03f * stacks;
        slow = Mathf.Min(slow, 0.30f);

        SlowEnemiesAround(transform.position, 2.5f, slow, 0.2f);
    }

    private void HandleReactiveArmorPlating()
    {
        int stacks = GetStacks(AbilityUpgradeType.ReactiveArmorPlating);
        if (stacks <= 0) return;

        reactiveArmorReady = true;

        reactiveArmorReduction = 0.05f * stacks;
        reactiveArmorReduction = Mathf.Min(reactiveArmorReduction, 0.50f);
    }

    private float ApplyReactiveArmorPlating(float damage)
    {
        if (!reactiveArmorReady)
        {
            return damage;
        }

        reactiveArmorReady = false;

        return damage * (1f - reactiveArmorReduction);
    }

    private void UpdateShieldedDiagnostics()
    {
        int stacks = GetStacks(AbilityUpgradeType.ShieldedDiagnostics);
        if (stacks <= 0) return;
        if (weapon == null) return;
        if (playerHP == null) return;
        if (!weapon.IsBlocking) return;

        if (Time.time - lastDamageTakenTime < 3f)
        {
            return;
        }

        float healRate = 0.25f * stacks;
        healRate = Mathf.Min(healRate, 2.5f);

        playerHP.Heal(healRate * Time.deltaTime);
    }

    private void HandleContactDebugger(EnemyHP enemy, bool isLightAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.ContactDebugger);
        if (stacks <= 0) return;
        if (enemy == null) return;
        if (!isLightAttack) return;

        contactMarks[enemy] = Time.time + 4f;
    }

    private float GetContactDebuggerMultiplier(EnemyHP enemy, bool isHeavyAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.ContactDebugger);
        if (stacks <= 0) return 1f;
        if (enemy == null) return 1f;
        if (!isHeavyAttack) return 1f;

        if (!contactMarks.ContainsKey(enemy)) return 1f;
        if (Time.time > contactMarks[enemy]) return 1f;

        float bonus = 0.15f + 0.03f * (stacks - 1);
        bonus = Mathf.Min(bonus, 0.42f);

        return 1f + bonus;
    }

    private void HandleUnstableMotorTiming(bool isLightAttack)
    {
        int stacks = GetStacks(AbilityUpgradeType.UnstableMotorTiming);
        if (stacks <= 0) return;
        if (weapon == null) return;
        if (!isLightAttack) return;

        float chance = 0.08f + 0.04f * (stacks - 1);
        chance = Mathf.Min(chance, 0.44f);

        if (Random.value <= chance)
        {
            weapon.ApplyTemporaryNextLightSpeedBoost(0.25f);
        }
    }

    private void DamageEnemiesAround(Vector3 center, float radius, float damage)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHP enemy = hit.GetComponentInParent<EnemyHP>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, gameObject);
            }
        }
    }

    private void SlowEnemiesAround(Vector3 center, float radius, float slowPercent, float duration)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyHP enemy = hit.GetComponentInParent<EnemyHP>();

            if (enemy != null)
            {
                enemy.SlowFor(slowPercent, duration);
            }
        }
    }

    private void CleanupContactMarks()
    {
        List<EnemyHP> removeList = new List<EnemyHP>();

        foreach (KeyValuePair<EnemyHP, float> pair in contactMarks)
        {
            if (pair.Key == null || Time.time > pair.Value)
            {
                removeList.Add(pair.Key);
            }
        }

        foreach (EnemyHP enemy in removeList)
        {
            contactMarks.Remove(enemy);
        }
    }
    public void ResetRunState()
    {
        terminationKillCount = 0;
        terminationKillWindowEndTime = 0f;
        terminationBuffEndTime = 0f;

        grazeCapacitorCharges = 0;

        storedImpactBonusDamage = 0f;
        retaliationEndTime = 0f;

        reactiveArmorReady = false;
        reactiveArmorReduction = 0f;
        armorChecksumReady = false;

        recursiveLastEnemy = null;
        recursiveHitCount = 0;

        weaponHitCounter = 0;

        contactMarks.Clear();

        Debug.Log("[RareCards] Run state reset.");
    }

}
