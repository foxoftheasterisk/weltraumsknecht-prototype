using Platformer.Core;
using Platformer.Model;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    void OnEnable()
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        model.spawnPoint = this.transform;
        model.player.transform.position = this.transform.position;
    }

}
