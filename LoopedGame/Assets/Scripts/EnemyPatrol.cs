using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using State = GenEnemyStates;

public enum GenEnemyStates
{
    SEARCHING,
    ATTACKING,
}

public class EnemyPatrol : MonoBehaviour
{
    public GameObject nodeHost;
    public GameObject target;
    public State enemyState;

    private NavMeshAgent enemyNav;
    private int node;
    private Transform[] points;
    private Transform targetPos;  
    private System.Random rand = new System.Random();
    private bool attacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
       points = nodeHost.GetComponentsInChildren<Transform>();
       enemyNav = GetComponent<NavMeshAgent>();
       Search();
       
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = target.GetComponent<Transform>();
        if (!enemyNav.pathPending && enemyNav.remainingDistance < 0.1f && !attacking) {
            Search();
        } 
        else if (Vector3.Distance(this.transform.position, targetPos.position) < 7) {
            AttackMode();
        } else
        {
            attacking = false;
        }


    }

    void Search()
    {
        enemyNav.destination = points[node].position;
        node = (node + rand.Next(0, 24)) % points.Length;
        enemyNav.stoppingDistance = 0;

    }
    
    
    public void AttackMode()
    {
        attacking = true;
        enemyNav.destination = targetPos.position; //target player
        enemyNav.stoppingDistance = 5;
    }
}
