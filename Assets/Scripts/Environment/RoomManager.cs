using Platformer.Core;
using Platformer.Model;
using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Door[] doors;

    public Collider2D cameraConfiner;

    private void Awake()
    {
        for(int i = 0; i < doors.Length; i++)
        {
            doors[i].ordinal = i;
        }

        CinemachineConfiner2D confinerScript = Simulation.GetModel<PlatformerModel>().virtualCamera.GetComponent<CinemachineConfiner2D>();
        confinerScript.BoundingShape2D = cameraConfiner;
        confinerScript.InvalidateBoundingShapeCache();

        FindAnyObjectByType<MapController>().StartRoom(this);
    }
}
