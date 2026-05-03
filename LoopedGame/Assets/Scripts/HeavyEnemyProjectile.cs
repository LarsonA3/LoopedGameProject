using Unity.VisualScripting;
using UnityEngine;

public class HeavyEnemyProjectile : EnemyProjectileBase
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public override void Setup(Vector3 direction, GameObject newShooter)
    {
        base.Setup(direction, newShooter);
    }

    public override void TryClear()
    {
        base.TryClear();
    }

    public override void TryReflect(float reflectedDamageMultiplier)
    {
        base.TryReflect(reflectedDamageMultiplier);
    }
}
