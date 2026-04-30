using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Tripolygon.UModelerX.Runtime;
using UnityEngine;
using UnityEngine.SubsystemsImplementation.Extensions;

public class Zone1Manager : MonoBehaviour
{
    public static Zone1Manager Instance;

    private int currentRoom;


    public GameObject rooms; //the "folder" under which all rooms are instantiated
    public GameObject player; //set to player capsule


    //room prefab load references
    public GameObject room1;
    public GameObject room2;


    public GameObject randomroom1;
    public GameObject randomroom2;
    public GameObject randomroom3;

    public GameObject zone2randomroom1;
    public GameObject zone2randomroom2;
    public GameObject zone2randomroom3;

    public GameObject zone3randomroom1;
    public GameObject zone3randomroom2;
    public GameObject zone3randomroom3;

    public GameObject zone4randomroom1;
    public GameObject zone4randomroom2;
    public GameObject zone4randomroom3;


    public GameObject finalroom;

    public GameObject startroomzone2;
    public GameObject startroomzone3;
    public GameObject startroomzone4;


    private int zone = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentRoom = 1;
        Instance = this;
        room1.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private GameObject roomToSwitchTo;
    public void nextRoom()
    {
        roomToSwitchTo = null;

        if (zone == 1)
        {
            switch (currentRoom)
            {
                case 1:
                    //go to room 2
                    print("switching to room 2");
                    roomToSwitchTo = room2;
                    currentRoom = 2;
                    break;

                //last room -1
                case 6:
                    print("went to final room");
                    //go to final room
                    roomToSwitchTo = finalroom;
                    break;

                //final room
                case 7:
                    print("going to next zone");
                    //go to next zone scene
                    break;



                //use random rooms for default case
                default:
                    //use random rooms for default case
                    int rand = Random.Range(1, 4); //1-3
                    print("switching to random room");
                    switch (rand)
                    {
                        case 1:
                            roomToSwitchTo = randomroom1;
                            break;
                        default:
                            print("issue with random rooms choice. maybe u went out of range on random");
                            roomToSwitchTo = randomroom1;
                            break;
                    }
                    currentRoom = currentRoom + 1;
                    break;
            }
        } // ##############################################
        else if (zone == 2)
        {
            switch (currentRoom)
            {



                //use random rooms for default case
                default:
                    //use random rooms for default case
                    int rand = Random.Range(1, 4); //1-3
                    print("switching to random room");
                    switch (rand)
                    {
                        case 1:
                            roomToSwitchTo = zone2randomroom1;
                            break;
                        default:
                            print("issue with random rooms choice. maybe u went out of range on random");
                            roomToSwitchTo = zone2randomroom1;
                            break;
                    }
                    currentRoom = currentRoom + 1;
                    break;
            }
        } // ##############################################
        else if (zone == 3)
        {
            switch (currentRoom)
            {

                //use random rooms for default case
                default:
                    //use random rooms for default case
                    int rand = Random.Range(1, 4); //1-3
                    print("switching to random room");
                    switch (rand)
                    {
                        case 1:
                            roomToSwitchTo = zone3randomroom1;
                            break;
                        default:
                            print("issue with random rooms choice. maybe u went out of range on random");
                            roomToSwitchTo = zone3randomroom1;
                            break;
                    }
                    currentRoom = currentRoom + 1;
                    break;
            }
        } // ##############################################
        else if (zone == 4)
        {
            switch (currentRoom)
            {

                //use random rooms for default case
                default:
                    //use random rooms for default case
                    int rand = Random.Range(1, 4); //1-3
                    print("switching to random room");
                    switch (rand)
                    {
                        case 1:
                            roomToSwitchTo = zone4randomroom1;
                            break;
                        default:
                            print("issue with random rooms choice. maybe u went out of range on random");
                            roomToSwitchTo = zone4randomroom1;
                            break;
                    }
                    currentRoom = currentRoom + 1;
                    break;
            }
        }

        //fade black screen
        //unload previous room (remove all under rooms)
        for (int i = rooms.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }
        //load new room (instantiate into rooms)
        GameObject newroom = Instantiate(roomToSwitchTo, rooms.transform, false);
        newroom.transform.localPosition = Vector3.zero;



        //set player location
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = Vector3.zero;
        Physics.SyncTransforms();
        if (cc != null) cc.enabled = true;

        //unfade black screen



    }



    public void nextZone ()
    {
        print("next zone...");

        if (zone != 4) { zone = zone + 1; }

        var roomToSwitchTo = zone switch
        {
            2 => startroomzone2,
            3 => startroomzone3,
            4 => startroomzone4,
            _ => null
        };

        if (roomToSwitchTo == null) return;



        //fade black screen
        //unload previous room (remove all under rooms)
        for (int i = rooms.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }


        //load new room (instantiate into rooms)
        GameObject newroom = Instantiate(roomToSwitchTo, rooms.transform, false);
        newroom.transform.localPosition = Vector3.zero;



        //set player location
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = Vector3.zero;
        Physics.SyncTransforms();
        if (cc != null) cc.enabled = true;

        //unfade black screen


        currentRoom = 1;

    }

}
