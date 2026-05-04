using UnityEngine;
using UnityEngine.AI;


public class EnemyPatrol : MonoBehaviour
{
    public GameObject nodeHost; // empty game object that contains all nodes the enemy will travel to (parent-inclusive)
    public GameObject target; //player
    private NavMeshAgent enemyNav;
    private int node;
    private Transform[] points;
    private Transform targetPos;  
    private System.Random rand = new System.Random();


    void Start()
    {
        
       points = nodeHost.GetComponentsInChildren<Transform>();
       enemyNav = GetComponent<NavMeshAgent>();
       //enemyNav.updateUpAxis = false; //prevents nav agent from changing up rotation of object
    }


    void Update()
    {
        targetPos = target.GetComponent<Transform>(); //update player position
        var distToPlayer = Vector3.Distance(this.transform.position, targetPos.position);
        if (!enemyNav.pathPending && enemyNav.remainingDistance < 0.1f && distToPlayer > 7) { //search smoothly if player is not in range
            Search();
        } 
        else {
            AttackMode();
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
        enemyNav.destination = targetPos.position; //target player
        enemyNav.stoppingDistance = 5;
    }
}
