using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public RoomDefinition startingRoom;

    private RoomDefinition currentRoomDefinition;
    private RoomManager currentRoomManager;

    // this feels very wrong but i'm not really seeing how else to do it
    private int doorToEnterAt = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Simulation.SetModel(this);
        SceneManager.LoadScene(startingRoom.scene, LoadSceneMode.Additive);
        currentRoomDefinition = startingRoom;

    }

    public void ExitThroughDoor(int door)
    {
        SceneManager.UnloadSceneAsync(currentRoomDefinition.scene);
        EnterRoom(currentRoomDefinition.doorDestinations[door]);
    }

    void EnterRoom(RoomDefinition.DoorAddress address)
    {
        currentRoomDefinition = address.room;
        SceneManager.LoadScene(currentRoomDefinition.scene, LoadSceneMode.Additive);
        doorToEnterAt = address.door;
    }

    public void StartRoom(RoomManager room)
    {
        currentRoomManager = room;
        if (doorToEnterAt >= 0)
        {
            PositionPlayerAtDoor(room.doors[doorToEnterAt]);
            doorToEnterAt = -1;
        }
    }

    void PositionPlayerAtDoor(Door door)
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        model.spawnPoint = door.transform;
        model.player.transform.position = door.transform.position;
    }


}
