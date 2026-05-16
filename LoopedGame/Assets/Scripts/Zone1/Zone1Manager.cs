using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zone1Manager : MonoBehaviour
{
    public static Zone1Manager Instance;

    private int currentRoom;
    private int intensity = 1;
    private int zone = 1;
    public GameObject enemies;

    [Header("Scene References")]
    public GameObject rooms; // The "folder" under which all rooms are instantiated.
    public GameObject player; // Set to player capsule.

    [Header("Zone 1 Rooms")]
    public GameObject room1Prefab;
    public GameObject room1;
    public GameObject room2;
    public GameObject randomroom1;
    public GameObject randomroom2;
    public GameObject randomroom3;
    public GameObject finalroom;

    [Header("Zone 2 Rooms")]
    public GameObject startroomzone2;
    public GameObject zone2randomroom1;
    public GameObject zone2randomroom2;
    public GameObject zone2randomroom3;
    public GameObject finalroom2;

    [Header("Zone 3 Rooms")]
    public GameObject startroomzone3;
    public GameObject zone3randomroom1;
    public GameObject zone3randomroom2;
    public GameObject zone3randomroom3;
    public GameObject finalroom3;

    [Header("Zone 4 Rooms")]
    public GameObject startroomzone4;
    public GameObject finalroom4;


    private GameObject roomToSwitchTo;



    public Transform playerVisualRoot;
    public GameObject tipUICanvas;
    public GameObject deathWhitePrefab;
    public float deathSequenceDuration = 2f;
    public GameObject gameUI;

    private void Start()
    {
        Instance = this;

        currentRoom = 1;
        zone = 1;
        ZoneBanner.Instance.Show(1);
        intensity = 1;

        roomToSwitchTo = room1Prefab != null ? room1Prefab : room1;

        if (roomToSwitchTo != null)
        {
            SwitchToRoom(roomToSwitchTo);
            MovePlayerToRoomStart();
        }
        ApplyDifficulty();
    }


    //DIFFICULTY STUFF
    private void ApplyDifficulty()
    {
        switch (DifficultySettings.Selected)
        {
            case Difficulty.Easy:
                break;
            case Difficulty.Medium:
                intensity += 1;
                break;
            case Difficulty.Hard:
                intensity += 3;
                break;
            case Difficulty.Nightmare:
                intensity += 6;
                break;
        }

        Debug.Log("[Zone1Manager] Difficulty: " + DifficultySettings.Selected + ", Starting intensity: " + intensity);
    }

    public void nextRoom()
    {
        roomToSwitchTo = null;

        if (zone == 1)
        {
            switch (currentRoom)
            {
                case 1:
                    print("switching to room 2");
                    roomToSwitchTo = room2;
                    currentRoom = 2;
                    break;

                case 6:
                    print("went to final room");
                    roomToSwitchTo = finalroom;
                    break;

                default:
                    roomToSwitchTo = GetRandomZone1Room();
                    currentRoom += 1;
                    break;
            }
        }
        else if (zone == 2)
        {
            switch (currentRoom)
            {
                case 6:
                    print("went to zone 2 final room");
                    roomToSwitchTo = finalroom2;
                    break;

                default:
                    roomToSwitchTo = GetRandomZone2Room();
                    currentRoom += 1;
                    break;
            }
        }
        else if (zone == 3)
        {
            switch (currentRoom)
            {
                case 6:
                    print("went to zone 3 final room");
                    roomToSwitchTo = finalroom3;
                    break;

                default:
                    roomToSwitchTo = GetRandomZone3Room();
                    currentRoom += 1;
                    break;
            }
        }
        else if (zone == 4)
        {
            switch (currentRoom)
            {
                case 1:
                    print("went to final room");
                    roomToSwitchTo = finalroom4;
                    currentRoom = 1000;
                    break;

                case 1000:
                    print("FINAL ROOM DETECTED");
                    return;

                default:
                    print("PLAYER WINS GAME");
                    resetRun();
                    return;
            }
        }

        if (roomToSwitchTo == null)
        {
            Debug.LogWarning("[Zone1Manager] roomToSwitchTo is null. Cannot switch rooms.");
            return;
        }

        intensity += 1;

        SwitchToRoom(roomToSwitchTo);
        MovePlayerToRoomStart();

        AddScore(100);
    }

    public void nextZone()
    {
        print("next zone...");

        if (zone != 4)
        {
            zone += 1;
            ZoneBanner.Instance.Show(zone);
        }

        roomToSwitchTo = zone switch
        {
            2 => startroomzone2,
            3 => startroomzone3,
            4 => startroomzone4,
            _ => null
        };

        if (roomToSwitchTo == null)
        {
            Debug.LogWarning("[Zone1Manager] No start room found for zone " + zone);
            return;
        }

        currentRoom = 1;
        intensity += 2;

        SwitchToRoom(roomToSwitchTo);
        MovePlayerToRoomStart();

        AddScore(500);
    }

    public int getIntensity()
    {
        return intensity;
    }

    public void resetRun()
    {
        HScore hScore = null;

        if (player != null)
        {
            hScore = player.GetComponentInChildren<HScore>();
        }

        if (hScore != null)
        {
            hScore.FinalScore();
            hScore.ResetScore();
        }

        zone = 1;
        currentRoom = 1;
        intensity = 0;

        roomToSwitchTo = room1Prefab != null ? room1Prefab : room1;

        if (roomToSwitchTo != null)
        {
            SwitchToRoom(roomToSwitchTo);
            MovePlayerToRoomStart();
        }
    }

    public void ResetAfterPlayerDeath()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Debug.Log("[Zone1Manager] Death sequence started.");

        // 1. Play tick sound
        SoundManager.PlaySound("4seconds");

        // 2. Clear enemies and room immediately
        for (int i = enemies.transform.childCount - 1; i >= 0; i--)
            Destroy(enemies.transform.GetChild(i).gameObject);

        for (int i = rooms.transform.childCount - 1; i >= 0; i--)
            Destroy(rooms.transform.GetChild(i).gameObject);

        // 3. Flash white
        GameObject whiteFlash = null;
        if (deathWhitePrefab != null)
            whiteFlash = Instantiate(deathWhitePrefab);

        // 4. Show tip
        if (tipUICanvas != null) tipUICanvas.SetActive(true);
        if (gameUI != null) gameUI.SetActive(true);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Health"))
            if (obj.transform.parent == null || obj.transform.parent.name != "DONOTDESTROY")
                Destroy(obj);

        // 5. Shake visual root for deathSequenceDuration
        if (playerVisualRoot != null)
        {
            Vector3 origin = playerVisualRoot.localPosition;
            float elapsed = 0f;
            float mag = 0.08f;

            while (elapsed < deathSequenceDuration)
            {
                elapsed += Time.deltaTime;
                playerVisualRoot.localPosition = origin + new Vector3(
                    Random.Range(-mag, mag),
                    Random.Range(-mag, mag),
                    Random.Range(-mag, mag)
                );
                yield return null;
            }

            playerVisualRoot.localPosition = origin;
        }
        else
        {
            yield return new WaitForSeconds(deathSequenceDuration);
        }

        // 6. Destroy white flash
        if (whiteFlash != null)
            Destroy(whiteFlash);

        // 7. Hide tip
        if (tipUICanvas != null) tipUICanvas.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        // ---- existing reset logic unchanged below ----

        Debug.Log("[Zone1Manager] Player death reset started.");
        Time.timeScale = 1f;

        zone = 1;
        ZoneBanner.Instance?.Show(1);
        currentRoom = 1;
        intensity = 1;

        HScore hScore = player != null ? player.GetComponentInChildren<HScore>() : null;
        if (hScore != null) { hScore.FinalScore(); hScore.ResetScore(); }

        roomToSwitchTo = room1Prefab != null ? room1Prefab : room1;
        if (roomToSwitchTo != null)
        {
            SwitchToRoom(roomToSwitchTo);
            MovePlayerToRoomStart();
        }
        else Debug.LogWarning("[Zone1Manager] Room1 is not assigned.");

        if (player != null)
        {
            PlayerHP hp = player.GetComponent<PlayerHP>();
            if (hp != null) hp.ReviveAt(Vector3.zero, hp.MaxHP);
        }

        Debug.Log("[Zone1Manager] Player death reset complete.");
    }

    private void SwitchToRoom(GameObject newRoomPrefab)
    {
        if (rooms == null)
        {
            Debug.LogWarning("[Zone1Manager] Rooms container is not assigned.");
            return;
        }

        for (int i = rooms.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }

        GameObject newRoom = Instantiate(newRoomPrefab, rooms.transform, false);
        newRoom.transform.localPosition = Vector3.zero;
    }

    private void MovePlayerToRoomStart()
    {
        if (player == null)
        {
            Debug.LogWarning("[Zone1Manager] Player is not assigned.");
            return;
        }

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = Vector3.zero;
        Physics.SyncTransforms();

        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    private GameObject GetRandomZone1Room()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                return randomroom1;

            case 2:
                return randomroom2;

            case 3:
                return randomroom3;

            default:
                return randomroom1;
        }
    }

    private GameObject GetRandomZone2Room()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                return zone2randomroom1;

            case 2:
                return zone2randomroom2;

            case 3:
                return zone2randomroom3;

            default:
                return zone2randomroom1;
        }
    }

    private GameObject GetRandomZone3Room()
    {
        int rand = Random.Range(1, 4);

        switch (rand)
        {
            case 1:
                return zone3randomroom1;

            case 2:
                return zone3randomroom2;

            case 3:
                return zone3randomroom3;

            default:
                return zone3randomroom1;
        }
    }

    private void AddScore(int amount)
    {
        if (player == null)
        {
            return;
        }

        HScore hScore = player.GetComponentInChildren<HScore>();

        if (hScore != null)
        {
            hScore.IncreaseScore(amount);
        }
    }
}