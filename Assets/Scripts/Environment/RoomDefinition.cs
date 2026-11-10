using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room")]
public class RoomDefinition : ScriptableObject
{
    public string scene;

    public DoorAddress[] doorDestinations;

    [Serializable]
    public struct DoorAddress
    {
        public RoomDefinition room;
        public int door;
    }

}
