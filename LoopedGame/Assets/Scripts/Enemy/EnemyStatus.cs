using System.Collections;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("Enemy Type")]
    [SerializeField] private bool isBoss = false;

    [Header("Stun")]
    [SerializeField] private bool isStunned = false;

    private Coroutine stunRoutine;

    public bool IsBoss => isBoss;
    public bool IsStunned => isStunned;

    public void TryStun(float duration)
    {
        if (isBoss)
        {
            return;
        }

        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }

        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunRoutine = null;
    }
}
