using UnityEngine;

public class RoomHandler : MonoBehaviour
{

    //script goes on each room
    //spawns enemies
    //checks if enemies dead
    //allows door to be gone through

    private Transform enemyContainer;
    private DoorNextRoom doorScript;
    private DoorGoNextZone goorGoNextZone;


    private GameObject player;
    private GameObject nodeHost;

    private bool isBoss = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get enemies 'folder' at root level
        GameObject enemyRoot = null;
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == "[[ Enemies ]]")
            {
                enemyRoot = root;
                break;
            }
        }

        if (enemyRoot != null)
            enemyContainer = enemyRoot.transform;
        else
            print("Could not find ([[ Enemies ]]) in root of scene pls place it there or dont delete it");


        // get player
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == "PLAYER") { player = root; break; }
        }
        if (player == null) print("PLAYER not found at scene root");

        // get this room's nodes
        Transform nodesTransform = transform.Find("NODES");
        if (nodesTransform != null)
            nodeHost = nodesTransform.gameObject;
        else
            print("No NODES child found under this room");




        // get door and close
        Transform doorTransform = transform.Find("Door");
        if (doorTransform != null)
        {
            doorScript = doorTransform.GetComponent<DoorNextRoom>();
            if (doorScript != null)
            {
                doorScript.Close();
                print("closed door successfully");
            }
            else
            {
                goorGoNextZone = doorTransform.GetComponent<DoorGoNextZone>();
                if (goorGoNextZone != null)
                {
                    goorGoNextZone.Close();
                    print("closed boss door successfully");
                }
            }
        }
        else
            print("DOOR IS NOT IN RIGHT SPOT UNDER ROOM");

        SpawnWave();
    }

    void Update()
    {
        if (enemyContainer == null) return;

        if (enemyContainer.childCount == 0)
        {
            //print("ENEMIES NOW EMPTY");

            if (doorScript != null && !doorScript.allowed)
                doorScript.Open();
            else if (goorGoNextZone != null && !goorGoNextZone.allowed)
                goorGoNextZone.Open();
        }
    }





    void SpawnWave()
    {
        print("spawning wave...");
        SpawnEnemy(1);
        //spawn wave based on intensity and ground tags
    }





    


    void SpawnEnemy(int enemyType)
    {
        string prefabName = enemyType switch
        {
            1 => "Slime",
            2 => "Cannon",
            3 => "Launcher",
            _ => null
        };

        if (prefabName == null) { print($"No prefab name for enemy type {enemyType}"); return; }

        GameObject prefab = Resources.Load<GameObject>($"Enemies/{prefabName}");

        if (prefab == null) { print($"Could not find prefab at Resources/Enemies/{prefabName}"); return; }
        if (player == null) { print("Cannot spawn enemy — player reference missing"); return; }
        if (nodeHost == null) { print("Cannot spawn enemy — nodeHost missing"); return; }

        Vector3 spawnPos;
        if (!TryGetSpawnPoint(out spawnPos))
        {
            print("Could not find valid spawn point on WALKABLE PLAYER FLOOR");
            return;
        }

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, enemyContainer);

        EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
        if (patrol != null)
        {
            patrol.nodeHost = nodeHost;
            patrol.target = player;
        }

        EnemyShooter shooter = enemy.GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.SetTarget(player.transform);
    }

    bool TryGetSpawnPoint(out Vector3 result, int maxAttempts = 30)
    {
        result = Vector3.zero;

        GameObject[] floors = GameObject.FindGameObjectsWithTag("WALKABLE PLAYER FLOOR");
        print($"Found {floors.Length} floor objects with tag");
        if (floors.Length == 0) return false;

        for (int i = 0; i < maxAttempts; i++)
        {
            Collider floorCol = floors[Random.Range(0, floors.Length)].GetComponent<Collider>();
            if (floorCol == null) { print($"Attempt {i}: no collider on floor object"); continue; }

            Bounds b = floorCol.bounds;
            Vector3 randomPoint = new Vector3(
                Random.Range(b.min.x, b.max.x),
                b.max.y + 1f,
                Random.Range(b.min.z, b.max.z)
            );

            if (!Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 5f))
            { print($"Attempt {i}: raycast missed entirely from {randomPoint}"); continue; }

            if (!hit.collider.CompareTag("WALKABLE PLAYER FLOOR"))
            { print($"Attempt {i}: raycast hit {hit.collider.name} tagged '{hit.collider.tag}' instead of floor"); continue; }

            Vector3 candidate = hit.point + Vector3.up * 0.1f;


            if (Vector3.Distance(candidate, player.transform.position) < 5f) continue; //make sure it isnt too close to plr

            LayerMask excludeFloor = ~(1 << hit.collider.gameObject.layer);
            if (Physics.CheckSphere(candidate, 0.5f, excludeFloor)) continue;

            result = candidate;
            return true;
        }

        return false;
    }
}

