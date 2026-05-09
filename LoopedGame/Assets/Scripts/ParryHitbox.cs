using UnityEngine;


public class ParryHitbox : MonoBehaviour
{
    [HideInInspector] public Weapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        if (weapon != null)
            weapon.OnParryContact(other);
    }
}