using Baerhous.Games.Towerfall.Enums;
using Unity.VisualScripting;
using UnityEngine;

namespace Baerhous.Games.Towerfall.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonInfo", menuName = "Dungeon/DungeonInfo")]
    public class DungeonInfo : ScriptableObject
    {
        public int seed;
        public DungeonClass dungeonClass;
        public int totalFloors;

        public GameObject[] startingRoomOptions;
        public GameObject[] rooms;
        public GameObject[] bossRoomOptions;
        public GameObject[] endCapRooms;

        DungeonInfo(){}

        public static DungeonInfo Create(int seed, DungeonClass dungeonClass, GameObject[] startingRoomOptions =null,
            GameObject[] rooms= null, GameObject[] bossRoomOptions= null)
        {
            DungeonInfo di = ScriptableObject.CreateInstance<DungeonInfo>();
            di.seed = seed;
            di.dungeonClass = dungeonClass;
            di.startingRoomOptions = startingRoomOptions;
            di.rooms = rooms;
            di.bossRoomOptions = bossRoomOptions;
            return di;
        }

        private static int _DetermineTotalFloors(DungeonClass dungeonClass)
        {
            switch (dungeonClass)
            {
                case DungeonClass.FClass:
                    return Random.Range(4, 6);
                case DungeonClass.EClass:
                    return Random.Range(6, 8);
                case DungeonClass.DClass:
                    return Random.Range(8, 10);
                case DungeonClass.CClass:
                    return Random.Range(10, 25);
                case DungeonClass.BClass:
                    return Random.Range(15, 30);
                case DungeonClass.AClass:
                    return Random.Range(20, 35);
                case DungeonClass.SClass:
                case DungeonClass.SsClass: 
                case DungeonClass.SssClass:
                    return Random.Range(25, 40);
                
            }
            return 0;
        }

        private void OnEnable()
        {
            if (seed == 0)
            {
                seed = Random.Range(1, 999999);
            }
            totalFloors = _DetermineTotalFloors(dungeonClass);
        }
    }
}