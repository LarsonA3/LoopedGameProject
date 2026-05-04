using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private EnemyProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;

    [Header("Timing")]
    [SerializeField] private float fireCooldown = 1.5f;
    [SerializeField] private bool fireAutomatically = true;

    private bool canFire = true;
    private EnemyStatus status;

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
    }

    private void Update()
    {
        if (!fireAutomatically) return;

        if (status != null && status.IsStunned) return;

        if (canFire)
        {
            FireAtTarget();
        }
    }

    public void FireAtTarget()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[EnemyShooter] No projectile prefab assigned.");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("[EnemyShooter] No fire point assigned.");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[EnemyShooter] No target assigned.");
            return;
        }

        Vector3 direction = target.position - firePoint.position;
        //Vector3 direction = firePoint.forward;
        direction.y = 0f;

        EnemyProjectileBase projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        projectile.Setup(direction, gameObject);

        StartCoroutine(FireCooldownRoutine());
    }

    private IEnumerator FireCooldownRoutine()
    {
        canFire = false;

        yield return new WaitForSeconds(fireCooldown);

        canFire = true;
    }
}
