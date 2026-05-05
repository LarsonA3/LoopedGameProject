using UnityEngine;

public class Zone1Manager : MonoBehaviour
{
    public static Zone1Manager Instance;

    private int currentRoom;
    private int intensity = 1;
    private int zone = 1;

    [Header("Scene References")]
    public GameObject rooms; // The "folder" under which all rooms are instantiated.
    public GameObject player; // Set to player capsule.

    [Header("Zone 1 Rooms")]
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

    private void Start()
    {
        Instance = this;

        currentRoom = 1;
        zone = 1;
        intensity = 1;

        if (room1 != null)
        {
            room1.SetActive(true);
        }
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

        UnlockWeaponForCompletedZone();

        if (zone != 4)
        {
            zone += 1;
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

        roomToSwitchTo = room1;

        if (roomToSwitchTo != null)
        {
            SwitchToRoom(roomToSwitchTo);
            MovePlayerToRoomStart();
        }

        StartingWeaponSelector selector =
            Object.FindFirstObjectByType<StartingWeaponSelector>(FindObjectsInactive.Include);

        if (selector != null)
        {
            selector.StartWeaponChoice();
        }
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

    private void UnlockWeaponForCompletedZone()
    {
        if (WeaponUnlockState.Instance == null)
        {
            Debug.LogWarning("[Zone1Manager] No WeaponUnlockState found.");
            return;
        }

        switch (zone)
        {
            case 1:
                WeaponUnlockState.Instance.UnlockWeapon(WeaponType.StunMace);
                break;

            case 2:
                WeaponUnlockState.Instance.UnlockWeapon(WeaponType.BootlegLightsaber);
                break;

            case 3:
                WeaponUnlockState.Instance.UnlockWeapon(WeaponType.GravityHammer);
                break;

            case 4:
                WeaponUnlockState.Instance.UnlockWeapon(WeaponType.KineticRiotShield);
                break;
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
