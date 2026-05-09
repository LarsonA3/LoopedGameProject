using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using State = EnemyFsm;

public enum EnemyFsm
{
    PATROL,
    ATTACK,
    KNOCKBACK,
}

public class EnemyPatrol : MonoBehaviour
{
    public State State { get; private set; }
    public GameObject nodeHost; // empty game object that contains all nodes the enemy will travel to (parent-inclusive)
    public GameObject target; //player
    private NavMeshAgent enemyNav;
    private Rigidbody rb;
    private int node;
    private Transform[] points;
    private Transform targetPos;
    private System.Random rand = new System.Random();
    private float distToPlayer;
    private Vector3 dirToPlayer;

    [Header("Knockback")]
    public float knockbackDuration = 0.6f;

    void Start()
    {
        State = State.PATROL;
        points = nodeHost.GetComponentsInChildren<Transform>();
        enemyNav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("[EnemyPatrol] Rigidbody required for knockback.", this);
        else
            rb.isKinematic = true; // NavMeshAgent drives movement by default

        node = (node + rand.Next(0, 24)) % points.Length; //avoids swarming behavior -- enemies go to the first node after spawning, which is always the player spawn location
    }

    void Update()
    {
        targetPos = target.GetComponent<Transform>(); //update player position
        distToPlayer = Vector3.Distance(gameObject.transform.position, targetPos.position);
        if (distToPlayer <= 5 && State == State.PATROL)
        {
            State = State.ATTACK;
        }

        switch (State)
        {
            case State.PATROL:
                Search();
                break;
            case State.ATTACK:
                GetComponent<EnemyShooter>().enable = true;
                AttackMode();
                dirToPlayer = (targetPos.position - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, dirToPlayer, Time.deltaTime * enemyNav.speed);
                break;
            case State.KNOCKBACK:
                // Physics is driving -- nothing to do here
                GetComponent<EnemyShooter>().enable = false;
                break;
        }
    }

    void Search()
    {
        if (!enemyNav.pathPending && enemyNav.remainingDistance < 1.6f) //if statement
        {
            enemyNav.destination = points[node].position;
            node = (node + rand.Next(0, 24)) % points.Length;
            enemyNav.stoppingDistance = 0;
        }
    }

    public void AttackMode()
    {
        enemyNav.destination = targetPos.position; //target player
        enemyNav.stoppingDistance = 6;
    }

    // Called by Weapon.cs on heavy hit
    public void TakeKnockback(Vector3 direction, float force)
    {
        if (State == State.KNOCKBACK) return; // already flying

        State = State.KNOCKBACK;

        // Hand control from NavMesh to physics
        enemyNav.enabled = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce((direction + Vector3.up * 0.4f) * force, ForceMode.VelocityChange);
        }

        StartCoroutine(Recoil());
    }

    IEnumerator Recoil()
    {
        yield return new WaitForSecondsRealtime(knockbackDuration);

        // Stop sliding, return control to NavMeshAgent
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Snap to nearest NavMesh point before re-enabling agent
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            transform.position = hit.position;

        enemyNav.enabled = true;
        State = State.ATTACK;
    }

    void OnCollisionEnter(Collision collision)
    {
        //colliding w/ another enemy
        if (collision.gameObject.tag == "Enemy" && State == State.KNOCKBACK)
        {
            collision.gameObject.GetComponent<EnemyHP>()?.TakeDamage(5f);
            print("knocked into enemy for 5 dmg");
        }
    }
}