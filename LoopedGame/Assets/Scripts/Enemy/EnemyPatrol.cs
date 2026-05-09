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
    public State State {get; private set;}
    public GameObject nodeHost; // empty game object that contains all nodes the enemy will travel to (parent-inclusive)
    public GameObject target; //player
    private NavMeshAgent enemyNav;
    private int node;
    private Transform[] points;
    private Transform targetPos;  
    private float force;
    private System.Random rand = new System.Random();
    private float distToPlayer;
    private Vector3 dirToPlayer;


    void Start()
    {
        State = State.PATROL;
        points = nodeHost.GetComponentsInChildren<Transform>();
        enemyNav = GetComponent<NavMeshAgent>();
        node = (node + rand.Next(0, 24)) % points.Length; //avoids swarming behavior -- enemies go to the first node after spawning, which is always the player spwan location
    }


    void Update()
    {
        targetPos = target.GetComponent<Transform>(); //update player position
        distToPlayer = Vector3.Distance(gameObject.transform.position, targetPos.position);


        if (distToPlayer <= 5)
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

    public void TakeKnockback(float knockback)
    {
        State = State.KNOCKBACK;
        force = knockback;
    }

    void OnCollisionExit(Collision collision)
    {
        //colliding w/ another enemy
        if (collision.gameObject.tag == "Enemy" && State == State.KNOCKBACK)
        {
            BroadcastMessage("TakeDamage", 5);
            print("knocked into enemy for 5 dmg");
        }
    }

    void OnTriggerStay(Collider other)
    {
        //hit with weapon
        if (State == State.KNOCKBACK && other.gameObject.tag == "Weapon")
        {
            //Vector3 direct = collision.contacts[0].point - gameObject.transform.position;
            //Vector3 direct = other.ClosestPoint(gameObject.transform.forward);
            //other.
            //direct = -direct.normalized;
            GetComponent<Rigidbody>().AddForce(other.transform.forward*force);
            Recoil();
            print("knocked back");
        }
    }

    IEnumerator Recoil()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        State = State.ATTACK;
    }

}
