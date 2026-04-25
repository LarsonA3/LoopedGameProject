using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SubsystemsImplementation.Extensions;

public class Zone1Manager : MonoBehaviour
{
    public static Zone1Manager Instance;

    private int currentRoom;


    public GameObject rooms; //the "folder" under which all rooms are instantiated
    public GameObject player;


    //room prefab load references
    public GameObject room1;
    public GameObject room2;


    public GameObject randomroom1;
    public GameObject randomroom2;
    public GameObject randomroom3;

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


        switch (currentRoom) {
            case 1:
                //go to room 2
                print("switching to room 2");
                roomToSwitchTo = room2;
                currentRoom = 2;
                break;

            //last room -1
            case 6:
                //go to final room

            //final room
            case 7:
                //go to next zone scene




            default:
                //use random rooms for default case
                print("error switching rooms, currentroom is " + currentRoom);
                break;

        }



        //fade black screen
        //unload previous room (remove all under rooms)
        for (int i = rooms.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }
        //load new room (instantiate into rooms)
        Instantiate(roomToSwitchTo, rooms.transform);
        //set player location
        player.transform.position = new Vector3(0, 0, 0);

        //unfade black screen
    }

}
