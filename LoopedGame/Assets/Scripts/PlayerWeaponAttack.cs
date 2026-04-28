using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponAttack : MonoBehaviour
{
    private Transform weaponTransform;
    private SphereCollider weaponHitbox;

    private float baseRange = 1.5f;
    private float attackArc = 0.4f; // dot product math pls like google before changing

    private float damage = 10f;
    private float attackCooldown = 0.35f;
    private LayerMask hitLayers = ~0; // everything by default

    private float currentRange;
    private bool canAttack = true;

    void Start()
    {
        Transform weaponChild = transform.Find("Weapon");
        if (weaponChild != null)
        {
            weaponTransform = weaponChild;
            weaponHitbox = weaponChild.GetComponent<SphereCollider>();
        }
        else
        {
            print("[Baton] ERROR — no child named 'Weapon' found!");
        }

        currentRange = baseRange;
        SyncHitboxRadius();
        print($"[Baton] Initialized — range: {currentRange}, arc: {attackArc}, damage: {damage}");
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) TryAttack();

        Gamepad pad = Gamepad.current;
        if (pad != null && pad.buttonWest.wasPressedThisFrame) TryAttack();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) UseAbility();
        if (pad != null && pad.buttonNorth.wasPressedThisFrame) UseAbility();
    }


    //atk

    void TryAttack()
    {
        if (!canAttack) { print("[Baton] on cooldown"); return; }
        PerformAttack();
    }

    void PerformAttack()
    {
        if (weaponTransform == null) { print("[Baton] ERROR — weaponTransform is null"); return; }

        canAttack = false;
        print($"[Baton] Attack fired — sphere at {weaponTransform.position}, radius {currentRange}");

        Collider[] hits = Physics.OverlapSphere(weaponTransform.position, currentRange, hitLayers);
        print($"[Baton] OverlapSphere hit {hits.Length} collider(s)");

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;       // skip self

            Vector3 dirToTarget = hit.transform.position - transform.position;
            dirToTarget.y = 0f;

            if (dirToTarget.sqrMagnitude < 0.001f) continue; // overlapping exactly, just skip

            float dot = Vector3.Dot(transform.forward, dirToTarget.normalized);
            print($"[Baton] {hit.gameObject.name} — dot: {dot:F2} (need >{attackArc})");
            if (dot < attackArc) continue;                    // not in swing arc

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);
            else
                print($"[Baton] {hit.gameObject.name} passed arc but has no IDamageable");

            print($"[Baton] Hit: {hit.gameObject.name}");
        }

        StartCoroutine(CooldownRoutine());
    }

    void UseAbility()
    {
        print("used active ability");
    }

    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        print("[Baton] Cooldown done — ready");
    }


    //upgrades to range here

    public void SetRange(float newRange)
    {
        print($"[Baton] Range: {currentRange} -> {newRange}");
        currentRange = newRange;
        SyncHitboxRadius();
    }

    public void ResetRange() => SetRange(baseRange);

    public void SetDamage(float newDamage) => damage = newDamage;

    public void SetAttackArc(float newArc) => attackArc = newArc;

    void SyncHitboxRadius()
    {
        if (weaponHitbox != null)
            weaponHitbox.radius = currentRange;
    }
}