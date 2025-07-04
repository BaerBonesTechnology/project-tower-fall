using System.Collections.Generic;
using UnityEngine;

namespace Baerhous.Games.Towerfall.Dungeons
{
    public class BaseDungeonRoom : MonoBehaviour
    {
        [Tooltip("Name of Room")] public string roomName;

        [Tooltip("Parent Game Object that holds all of the collision boxes for the room ")]
        public GameObject roomCollisionBox;

        [Tooltip("Parent GameObject holding all of the exit points for the room")]
        public Transform[] roomExits;

        [Tooltip("spawn points for enemies")]
        public Transform[] enemySpawns;

        [Tooltip("Parent GameObject holding all of the spawn points for found loot")]
        public Transform[] lootSpawns;

        [Tooltip("Check if room is a connection room to ensure it has an actual room attached to it")]
        public bool isConnectionRoom;

        /// <summary>
        /// Gets all the child transforms under the roomExits parent.
        /// </summary>
        /// <returns> A List of all the Room Exits </returns>
        public List<Transform> GetRoomExits()
        {
            var exits = new List<Transform>();
            if (roomExits != null)
            {
                foreach (Transform exit in roomExits)
                {
                    exits.Add(exit.transform);
                }
            }

            return exits;
        }
    }
}