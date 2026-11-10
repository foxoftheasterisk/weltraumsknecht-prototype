using Platformer.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public RoomDefinition startingRoom;

    private RoomDefinition currentRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Simulation.SetModel(this);
        SceneManager.LoadScene(startingRoom.scene, LoadSceneMode.Additive);
        currentRoom = startingRoom;

    }

    public void ExitThroughDoor(int door)
    {
        SceneManager.UnloadSceneAsync(currentRoom.scene);
        EnterRoom(currentRoom.doorDestinations[door]);
    }

    void EnterRoom(RoomDefinition.DoorAddress address)
    {
        currentRoom = address.room;
        SceneManager.LoadScene(currentRoom.scene, LoadSceneMode.Additive);
        //Move player to correct door


    }


}
