using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class KineticRiotShieldWeapon : WeaponBase
{
    [Header("Blocking")]
    [SerializeField] private float maxStoredEnergy = 100f;
    [SerializeField] private float storedEnergy;
    [SerializeField] private bool isBlocking;

    [Header("Shield Throw")]
    [SerializeField] private GameObject shieldProjectilePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float baseThrowDamage = 20f;
    [SerializeField] private float energyDamageMultiplier = 0.5f;
    [SerializeField] private float throwCooldown = 3f;

    private bool canThrow = true;

    public bool IsBlocking => isBlocking;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            StartBlocking();
        }

        if (Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame)
        {
            StopBlocking();
        }
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!canThrow)
        {
            Debug.Log("[Kinetic Riot Shield] Throw on cooldown.");
            return;
        }

        if (shieldProjectilePrefab == null)
        {
            Debug.LogWarning("[Kinetic Riot Shield] No shield projectile prefab assigned.");
            return;
        }

        if (throwPoint == null)
        {
            Debug.LogWarning("[Kinetic Riot Shield] No throw point assigned.");
            return;
        }

        StartCoroutine(ThrowShieldRoutine());
    }

    public void StartBlocking()
    {
        isBlocking = true;
    }

    public void StopBlocking()
    {
        isBlocking = false;
    }

    public void AbsorbHit(float incomingDamage)
    {
        if (!isBlocking)
        {
            return;
        }

        storedEnergy += incomingDamage;
        storedEnergy = Mathf.Clamp(storedEnergy, 0f, maxStoredEnergy);
    }

    private IEnumerator ThrowShieldRoutine()
    {
        canThrow = false;

        GameObject shieldObject = Instantiate(
            shieldProjectilePrefab,
            throwPoint.position,
            throwPoint.rotation
        );

        BouncingShieldProjectile projectile = shieldObject.GetComponent<BouncingShieldProjectile>();

        if (projectile != null)
        {
            float finalDamage = baseThrowDamage + storedEnergy * energyDamageMultiplier;

            projectile.Setup(
                GetForwardDirection(),
                finalDamage,
                EnemyLayer
            );
        }

        storedEnergy = 0f;

        yield return new WaitForSeconds(GetFinalSpecialCooldown(throwCooldown));

        canThrow = true;
    }
}
