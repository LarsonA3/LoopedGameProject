using UnityEngine;

public class ParriedProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 18f;
    public float lifetime = 4f;

    [Header("Damage")]
    public float damage = 5f;

    // Set by Weapon.ReflectProjectile() immediately after Instantiate.
    [HideInInspector] public Vector3 direction = Vector3.forward;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHP hp = other.GetComponentInParent<EnemyHP>();

        if (hp != null)
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Player") || other.CompareTag("Weapon"))
            return;

        Destroy(gameObject);
    }
}