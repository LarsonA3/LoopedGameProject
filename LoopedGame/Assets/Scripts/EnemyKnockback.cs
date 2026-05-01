using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (rb == null) return;

        direction.y = 0f;
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }
}
