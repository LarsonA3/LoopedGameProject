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
            print("ENEMIES NOW EMPTY");

            if (doorScript != null && !doorScript.allowed)
                doorScript.Open();
            else if (goorGoNextZone != null && !goorGoNextZone.allowed)
                goorGoNextZone.Open();
        }
    }



    void SpawnWave()
    {
        print("spawning wave...");

        //spawn wave based on intensity and ground tags
    }
}

