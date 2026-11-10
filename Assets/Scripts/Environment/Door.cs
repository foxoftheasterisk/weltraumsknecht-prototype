using Platformer.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weltraumsknecht.Player;

public class Door : MonoBehaviour
{
    [DoNotSerialize]
    public int ordinal;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Simulation.GetModel<MapController>().ExitThroughDoor(ordinal);
    }
}
