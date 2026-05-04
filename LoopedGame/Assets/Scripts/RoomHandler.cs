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




    // uses kinda point buy system based on room intensity.
    // slime = 1 point, cannon = 2 points, launcher = 3 points
    void SpawnWave()
    {
        print("spawning wave...");

        int intensity = Zone1Manager.Instance.getIntensity();
        int budget = intensity;

        //added wieghts here for variety
        var enemyTable = new (int type, int cost, int weight, int unlock)[]
        {
        (1, 1, 40, 1),
        (2, 2, 35, 3),
        (3, 3, 25, 5),
        };

        System.Collections.Generic.List<int> spawnedTypes = new();

        int attempts = 0;
        while (budget > 0 && attempts < 50)
        {
            attempts++;

            // build affordable + unlocked pool with weights
            var pool = new System.Collections.Generic.List<(int type, int weight)>();
            foreach (var e in enemyTable)
            {
                if (intensity < e.unlock) continue;
                if (e.cost > budget) continue;

                // boost slime weight if havent spawned one yet this wave
                int w = e.type == 1 && !spawnedTypes.Contains(1) ? e.weight + 20 : e.weight;
                pool.Add((e.type, w));
            }

            if (pool.Count == 0) break;

            //weighted rand pick
            int totalWeight = 0;
            foreach (var p in pool) totalWeight += p.weight;

            int roll = Random.Range(0, totalWeight);
            int chosen = pool[0].type;
            int cursor = 0;
            foreach (var p in pool)
            {
                cursor += p.weight;
                if (roll < cursor) { chosen = p.type; break; }
            }

            int chosenCost = chosen switch { 1 => 1, 2 => 2, 3 => 3, _ => 99 };

            SpawnEnemy(chosen);
            spawnedTypes.Add(chosen);
            budget -= chosenCost;
        }
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

        // snap to navmesh surface
        UnityEngine.AI.NavMeshHit navHit;
        if (UnityEngine.AI.NavMesh.SamplePosition(enemy.transform.position, out navHit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            enemy.transform.position = navHit.position;

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

