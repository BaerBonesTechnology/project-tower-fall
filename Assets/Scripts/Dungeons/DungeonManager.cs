using System;
using System.Collections.Generic;
using Baerhous.Games.Towerfall.Enums;
using Baerhous.Games.Towerfall.Exceptions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Baerhous.Games.Towerfall.Dungeons
{
     /// <summary>
    /// Manages the world's available dungeons by creating random generated `DungeonInfo` instances
    /// </summary>
    public class DungeonManager : MonoBehaviour
    {
        public List<DungeonInfo> availableDungeons;
        public GameObject[] TestRooms;

        enum DungeonType
        {
            Test,
        }

        private void Start()
        {
            _GenerateAllDungeons();
        }
        
      private void _GenerateAllDungeons()
        {
            foreach (DungeonClass dungeonClass in Enum.GetValues(typeof(DungeonClass)))
            {
                for (int i = 0; i < 6; i++)
                {
                    _CreateRandomDungeon(dungeonClass);
                }
            }
        }
      
        private void _CreateRandomDungeon(DungeonClass forcedClass)
        {
            int seed;
            DungeonType type = _GetRandomType();
            GameObject[] startingRoomOptions;
            GameObject[] roomOptions;
            GameObject[] bossRoomOptions;
            DungeonClass nextDungeonClass = forcedClass;

            try
            {
                seed = Random.Range(0, 100000);
                switch (type)
                {
                    case DungeonType.Test:
                        startingRoomOptions = TestRooms;
                        roomOptions = TestRooms;
                        bossRoomOptions = TestRooms;
                        break;
                    default:
                        throw new InvalidDungeonException();
                }

                DungeonInfo newInfo = DungeonInfo.Create(
                    seed: seed,
                    dungeonClass: nextDungeonClass,
                    startingRoomOptions: startingRoomOptions,
                    rooms: roomOptions,
                    bossRoomOptions: bossRoomOptions
                );

                availableDungeons.Add(newInfo);
            }
            catch (DungeonException)
            {
                Debug.Log("Dungeon Generation Complete");
            }
            catch (InvalidDungeonException)
            {
                Debug.Log("Dungeon attempted to generate invalid dungeon type");
            }
        }
        
  

        /// <summary>
        /// Makes sure that only 6 quests per class are generated
        /// </summary>
        /// <returns></returns>
        private DungeonClass _ResolveClass(DungeonClass checkClass = DungeonClass.FClass)
        {
            Debug.Log("Resolving Class: " + checkClass);
            
            if (availableDungeons.Count > 1)
            {
                DungeonInfo[] alreadyCreated =
                    availableDungeons.FindAll((info) => info.dungeonClass == checkClass).ToArray();
                
                // if already created is 6 create the next class up
                if (alreadyCreated.Length >= 6 && checkClass != DungeonClass.SsClass)
                {
                    _ResolveClass(checkClass++);
                } else if (alreadyCreated.Length >= 6 &&  checkClass == DungeonClass.SsClass)
                {
                    throw new DungeonException("Dungeon Info Generation Complete");
                }
                return checkClass;
            }
            return checkClass;
        }
        
        private DungeonType _GetRandomType(){
            // TODO: Flesh out enums for types and get random between the total length of types.
            // TODO: In future some dungeon types will be locked by level
            
            return DungeonType.Test;
        }
    }
}