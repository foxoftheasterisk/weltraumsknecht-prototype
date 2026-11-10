using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        //...what the fuck.
        //I'm sorry but that is a BULLSHIT way for that to work.
        //"Oh yeah just get this reference without ever creating it, it's fine!"
        //Anyway it maybe isn't working now.
        //Possibly because i'm now using it in multiple scenes so they're maybe trying to share the instance even though that doesn't work?
        //Ugh, I'm going to have to rework this whole system aren't I.
        //...well, it would probably be fine IF we just have GameController in a singleton "managers" scene. And then additively load the actual play area scenes.
        //Which I GUESS is probably what we want to do anyway?
        //...still though, this is bullshit.
        //Doesn't even have a warning that you shouldn't have more than one in a project.


        void OnEnable()
        {
            Instance = this;
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void LateUpdate()
        {
            if (Instance == this) Simulation.Tick();
        }
    }
}