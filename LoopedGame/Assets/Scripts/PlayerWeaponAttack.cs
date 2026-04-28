using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponAttack : MonoBehaviour
{
    private Transform weaponTransform;       // drag Weapon object here
    private SphereCollider weaponHitbox;     // sphere collider on Weapon

    private float baseRange = 1.5f;
    private float attackArc = 0.4f;          // dot product math pls like google before changing


    private float damage = 10f;
    private float attackCooldown = 0.35f;
    private LayerMask hitLayers = ~0;        // everything by default

    private float currentRange;
    private bool canAttack = true;

    void Start()
    {
        currentRange = baseRange;
        SyncHitboxRadius();
    }


    public void OnAttack(InputValue value)
    {
        if (!value.isPressed || !canAttack) return;
        PerformAttack();
    }

    public void OnAbility(InputValue value)
    {
        if (!value.isPressed) return;
        UseAbility();
    }

  


    //atk

    void PerformAttack()
    {
        canAttack = false;

        Collider[] hits = Physics.OverlapSphere(weaponTransform.position, currentRange, hitLayers);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;       // skip self

            Vector3 dirToTarget = hit.transform.position - transform.position;
            dirToTarget.y = 0f;                               // flatten — top-down facing

            if (dirToTarget.sqrMagnitude < 0.001f) continue; // overlapping exactly, just skip

            float dot = Vector3.Dot(transform.forward, dirToTarget.normalized);
            if (dot < attackArc) continue;                    // not in swing arc

            IDamageable damageable = hit.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);

            Debug.Log($"[Baton] Hit: {hit.gameObject.name}");
        }

        StartCoroutine(CooldownRoutine());
    }

    void UseAbility()
    {
        Debug.Log("used active ability");
    }

    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }



    //upgrades to range here

    public void SetRange(float newRange)
    {
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