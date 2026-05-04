using UnityEngine;

public class BouncingEnemyProjectile : EnemyProjectileBase
{
    [Header("Bounce Settings")]
    [SerializeField] private int maxBounces = 3;

    private int currentBounces;

    protected override void TryHitWall(Collider other)
    {
        bool hitWall = ((1 << other.gameObject.layer) & wallLayer) != 0;

        if (!hitWall) return;

        currentBounces++;

        Vector3 closestPoint = other.ClosestPoint(transform.position);
        Vector3 normal = transform.position - closestPoint;
        normal.y = 0f;

        if (normal.sqrMagnitude < 0.01f)
        {
            normal = -moveDirection;
        }

        moveDirection = Vector3.Reflect(moveDirection, normal.normalized).normalized;

        if (currentBounces >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
}
