using System.Collections;
using UnityEngine;

public class StunMaceWeapon : WeaponBase
{
    [Header("Detached Mace Head Special")]
    [SerializeField] private GameObject maceHeadPrefab;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private float detachedDamage = 35f;
    [SerializeField] private float detachedLaunchSpeed = 15f;
    [SerializeField] private float detachedReturnSpeed = 20f;
    [SerializeField] private float detachedTravelTime = 0.5f;
    [SerializeField] private float detachCooldown = 3f;

    [Header("Stun")]
    [SerializeField] private float stunChance = 0.35f;
    [SerializeField] private float stunDuration = 1.5f;

    private bool canDetach = true;

    public override void Attack()
    {
        base.Attack();
    }

    public override void SpecialAttack()
    {
        if (!canDetach)
        {
            Debug.Log("[Stun Mace] Head launch is on cooldown.");
            return;
        }

        if (maceHeadPrefab == null)
        {
            Debug.LogWarning("[Stun Mace] No mace head prefab assigned.");
            return;
        }

        if (launchPoint == null)
        {
            Debug.LogWarning("[Stun Mace] No LaunchPoint assigned.");
            return;
        }

        StartCoroutine(DetachHeadRoutine());
    }

    private IEnumerator DetachHeadRoutine()
    {
        canDetach = false;

        GameObject maceHeadObject = Instantiate(
            maceHeadPrefab,
            launchPoint.position,
            launchPoint.rotation
        );

        ReturningMaceHead maceHead = maceHeadObject.GetComponent<ReturningMaceHead>();

        if (maceHead != null)
        {
            maceHead.Setup(
                GetForwardDirection(),
                launchPoint,
                detachedDamage,
                detachedLaunchSpeed,
                detachedReturnSpeed,
                detachedTravelTime,
                stunChance,
                stunDuration,
                EnemyLayer
            );
        }

        yield return new WaitForSeconds(GetFinalSpecialCooldown(detachCooldown));

        canDetach = true;
    }
}
