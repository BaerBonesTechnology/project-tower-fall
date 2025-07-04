using System.Collections;
                        using System.Collections.Generic;
                        using System.Linq;
                        using Unity.VisualScripting;
                        using UnityEditor;
                        using UnityEngine;
                        
                        namespace Baerhous.Games.Towerfall.Dungeons
                        {
                            public interface IDungeonGenerator
                            {
                                void InitializeDungeon();
                                void ResetDungeon();
                            }
                        
                            public class DungeonGenerator : MonoBehaviour, IDungeonGenerator
                            {
                                [Header("Dungeon Settings")] [SerializeField]
                                private DungeonInfo dungeonInfo;
                        
                                [SerializeField] private bool isBossDungeon;
                                [SerializeField] private int dungeonSeed;
                                [Tooltip("When debug is true, dungeon generation will slow down completely")]
                                [SerializeField] private bool debug;
                        
                                [Header("Generation Settings")] [SerializeField]
                                private int maxOverlapRetries = 10;
                        
                                private readonly List<BaseDungeonRoom> _placedRooms = new();
                                private readonly Dictionary<BaseDungeonRoom, List<Transform>> _availableExits = new();
                                private BaseDungeonRoom _chosenExitRoom;
                                private BaseDungeonRoom _lastPlacedRoom;
                                private Transform _selectedExit;
                                private int _currentRoomCount;
                                private int _overlapRetryCount;
                        
                                private DungeonDebugger _debugger;
                        
                                private void Awake()
                                {
                                    _debugger = new DungeonDebugger(this);
                                }
                        
                                private void Start()
                                {
                                    InitializeDungeon();
                                }
                        
                                public void InitializeDungeon()
                                {
                                    if (!ValidateDungeonInfo()) return;
                        
                                    InitializeRandomSeed();
                                    ResetDungeon();
                                    DetermineDungeonSize();
                                    GenerateDungeon();
                                }
                        
                                private bool ValidateDungeonInfo()
                                {
                                    if (dungeonInfo) return true;
                        
                                    Debug.LogError("DungeonInfo is not set. Cannot generate dungeon.");
                                    return false;
                                }
                        
                                private void InitializeRandomSeed()
                                {
                                    dungeonSeed = dungeonInfo.seed;
                                    Random.InitState(dungeonSeed);
                                }
                        
                                private void GenerateDungeon()
                                {
                                    Debug.Log($"GenerateDungeon called. Seed: {dungeonSeed}. Target Rooms: {dungeonInfo.totalFloors}");
                                    GenerateStartingRoom();
                                    
                                    // Main generation loop
                                    while (!IsDungeonComplete() && HasAvailableExits())
                                    {
                                        if (debug)
                                        {
                                            _debugger.ApplyGenerationDelay();
                                        }
                                        if (ShouldGenerateBossRoom())
                                        {
                                            GenerateBossRoom();
                                        }
                                        else
                                        {
                                            GenerateNextRoom();
                                        }
                                    }
                        
                                    AddEndsToConnectionRooms();
                                    Debug.Log("Dungeon generation process finished.");
                                }
                        
                                private void GenerateStartingRoom()
                                {
                                    if (!ValidateStartingRoomOptions()) return;
                        
                                    var roomPrefab = GetRandomRoom(dungeonInfo.startingRoomOptions);
                                    
                                    // Retry loop for starting room
                                    int attempts = 0;
                                    while(attempts < maxOverlapRetries)
                                    {
                                        SpawnRoom(roomPrefab, transform.position, transform.rotation);
                                        if (ProcessSpawnedRoom())
                                        {
                                            return; // Success
                                        }
                                        attempts++;
                                    }
                                    Debug.LogError("Failed to place starting room without overlap.");
                                }
                        
                                private bool ValidateStartingRoomOptions()
                                {
                                    if (dungeonInfo.startingRoomOptions.Length != 0) return true;
                        
                                    Debug.LogError("No starting room options available in DungeonInfo.");
                                    return false;
                                }
                        
                                private void GenerateNextRoom()
                                {
                                    _overlapRetryCount = 0;
                                    while (_overlapRetryCount < maxOverlapRetries)
                                    {
                                        if (!HasAvailableExits())
                                        {
                                            Debug.LogWarning("Ran out of exits during generation attempt.");
                                            return;
                                        }
                        
                                        SelectAndRemoveRandomExit();
                                        if (_selectedExit is null) continue;
                        
                                        var roomPrefab = GetRandomRoom(dungeonInfo.rooms);
                                        SpawnRoom(roomPrefab, _selectedExit.position, _selectedExit.rotation);
                        
                                        _debugger.ApplyGenerationDelay();
                                        if (ProcessSpawnedRoom())
                                        {
                                            return; // Successfully placed a room
                                        }
                                        // If ProcessSpawnedRoom returns false, it's a failure, loop will retry.
                                    }
                                    Debug.LogWarning("Exceeded max retries for a single room placement. Moving to another exit.");
                                }
                        
                                private void GenerateBossRoom()
                                {
                                    if (!HasAvailableExits()) return;
                        
                                    SelectAndRemoveRandomExit();
                                    var roomPrefab = GetRandomRoom(dungeonInfo.bossRoomOptions);
                                    SpawnRoom(roomPrefab, _selectedExit.position, _selectedExit.rotation);
                                    if (ProcessSpawnedRoom())
                                    {
                                        // Boss room is the last room
                                        _currentRoomCount = dungeonInfo.totalFloors;
                                    }
                                }
                        
                                private void SelectAndRemoveRandomExit()
                                {
                                    if (_availableExits.Count == 0)
                                    {
                                        _selectedExit = null;
                                        _chosenExitRoom = null;
                                        return;
                                    }
                        
                                    int randRoomKey = Random.Range(0, _availableExits.Keys.Count);
                                    _chosenExitRoom = _availableExits.Keys.ElementAt(randRoomKey);
                                    
                                    if (_availableExits[_chosenExitRoom].Count == 0)
                                    {
                                        _availableExits.Remove(_chosenExitRoom);
                                        SelectAndRemoveRandomExit(); // Recurse to find a valid exit
                                        return;
                                    }
                        
                                    int randExitIndex = Random.Range(0, _availableExits[_chosenExitRoom].Count);
                                    _selectedExit = _availableExits[_chosenExitRoom][randExitIndex];
                        
                                    _availableExits[_chosenExitRoom].RemoveAt(randExitIndex);
                        
                                    if (_availableExits[_chosenExitRoom].Count == 0)
                                    {
                                        _availableExits.Remove(_chosenExitRoom);
                                    }
                                }
                        
                                private GameObject GetRandomRoom(GameObject[] roomOptions)
                                {
                                    if (roomOptions == null || roomOptions.Length == 0) return null;
                                    var randomIndex = Random.Range(0, roomOptions.Length);
                                    return roomOptions[randomIndex];
                                }
                        
                                private void SpawnRoom(GameObject roomPrefab, Vector3 position, Quaternion rotation)
                                {
                                    if (roomPrefab is null)
                                    {
                                        Debug.LogWarning("Room prefab is null.");
                                        _lastPlacedRoom = null;
                                        return;
                                    }
                        
                                    var newRoom = Instantiate(roomPrefab, position, rotation, transform);
                                    _lastPlacedRoom = newRoom.GetComponent<BaseDungeonRoom>();
                                }
                        
                                private bool ProcessSpawnedRoom()
                                {
                                    if (_lastPlacedRoom is null)
                                    {
                                        Debug.LogWarning("Last placed room is null.");
                                        HandleFailedPlacement();
                                        return false;
                                    }
                        
                                    if (CheckForOverlap(_lastPlacedRoom))
                                    {
                                        Debug.LogWarning($"Overlap detected for room {_lastPlacedRoom.name}.");
                                        HandleFailedPlacement();
                                        return false;
                                    }
                                    
                                    if (_lastPlacedRoom.isConnectionRoom && !HasTrueConnection())
                                    {
                                        Debug.LogWarning($"Invalid connection for room {_lastPlacedRoom.name}.");
                                        HandleFailedPlacement();
                                        return false;
                                    }
                        
                                    HandleSuccessfulPlacement();
                                    return true;
                                }
                        
                                private void HandleFailedPlacement()
                                {
                                    if (_lastPlacedRoom)
                                    {
                                        Destroy(_lastPlacedRoom.gameObject);
                                        _lastPlacedRoom = null;
                                    }
                                    _overlapRetryCount++;
                                }
                        
                                private void HandleSuccessfulPlacement()
                                {
                                    _placedRooms.Add(_lastPlacedRoom);
                                    _currentRoomCount++;
                                    
                                    var exits = _lastPlacedRoom.GetRoomExits();
                                    // For connection rooms, only add exits that are not the one we entered from
                                    if (_lastPlacedRoom.isConnectionRoom && _selectedExit)
                                    {
                                        exits.RemoveAll(exit => Vector3.Distance(exit.position, _selectedExit.position) < 0.1f);
                                    }
                        
                                    if(exits.Count > 0)
                                    {
                                        _availableExits.Add(_lastPlacedRoom, exits);
                                    }
                        
                                    Debug.Log($"Placed room {_currentRoomCount}/{dungeonInfo.totalFloors}: {_lastPlacedRoom.name}. Available exits: {GetExitCount()}");
                                    _overlapRetryCount = 0;
                                }
                        
                                private bool ShouldGenerateBossRoom()
                                {
                                    return isBossDungeon && _currentRoomCount == dungeonInfo.totalFloors - 1;
                                }
                        
                                private bool HasAvailableExits()
                                {
                                    return _availableExits.Values.Any(exitList => exitList.Count > 0);
                                }
                        
                                private bool CheckForOverlap(BaseDungeonRoom room)
                                {
                                    if (room.roomCollisionBox)
                                    {
                                        var colliders = room.roomCollisionBox.GetComponentsInChildren<Collider>();
                                        foreach (var thisCollider in colliders)
                                        {
                                            Collider[] results = new List<Collider>().ToArray();
                                            var size = Physics.OverlapBoxNonAlloc(thisCollider.bounds.center, thisCollider.bounds.extents, results, thisCollider.transform.rotation);

                                            if (size > 0) return true;
                                        }
                                    }
                                    return false;
                                }
                        
                                private bool HasOverlappingRoom(Collider[] overlappingColliders, BaseDungeonRoom room)
                                {
                                    foreach (var overlappingCollider in overlappingColliders)
                                    {
                                        if (overlappingCollider.transform.IsChildOf(room.transform)) continue;
                        
                                        var parentRoom = overlappingCollider.GetComponentInParent<BaseDungeonRoom>();
                                        if (parentRoom && _placedRooms.Contains(parentRoom))
                                        {
                                            return true;
                                        }
                                    }
                                    return false;
                                }
                        
                                private bool IsDungeonComplete()
                                {
                                    return _currentRoomCount >= dungeonInfo.totalFloors;
                                }
                        
                                public void ResetDungeon()
                                {
                                    StopAllCoroutines();
                        
                                    foreach (var room in _placedRooms)
                                    {
                                        if (room) Destroy(room.gameObject);
                                    }
                        
                                    var orphanedRooms = GetComponentsInChildren<BaseDungeonRoom>();
                                    foreach (var room in orphanedRooms)
                                    {
                                        if (room && !_placedRooms.Contains(room))
                                        {
                                            Destroy(room.gameObject);
                                        }
                                    }
                        
                                    _placedRooms.Clear();
                                    _availableExits.Clear();
                                    _currentRoomCount = 0;
                                    _overlapRetryCount = 0;
                                    _lastPlacedRoom = null;
                                    _chosenExitRoom = null;
                                    _selectedExit = null;
                                }
                        
                                private void DetermineDungeonSize()
                                {
                                    dungeonInfo.totalFloors = Random.Range(10, 20);
                                }
                        
                                private void AddEndsToConnectionRooms()
                                {
                                    if (dungeonInfo.endCapRooms == null || dungeonInfo.endCapRooms.Length == 0) return;
                        
                                    var connectionExits = new Dictionary<Transform, BaseDungeonRoom>();
                                    foreach (var room in _placedRooms)
                                    {
                                        if (_availableExits.TryGetValue(room, out var availableExit))
                                        {
                                            foreach (Transform exit in availableExit)
                                            {
                                                    connectionExits.Add(exit, room);
                                            }
                                        }
                                    }
                        
                                    foreach (var pair in connectionExits)
                                    {
                                        GameObject endCap = GetRandomRoom(dungeonInfo.endCapRooms);
                                        SpawnRoom(endCap, pair.Key.position, pair.Key.rotation);
                                        if (_lastPlacedRoom)
                                        {
                                            _placedRooms.Add(_lastPlacedRoom);
                                        }
                                    }
                                }
                        
                                private bool HasTrueConnection()
                                {
                                    if (_lastPlacedRoom is null || !_lastPlacedRoom.isConnectionRoom || _chosenExitRoom is null || _selectedExit is null)
                                    {
                                        return true; // Not a connection scenario we need to validate
                                    }
                        
                                    // A connection room cannot connect to another connection room directly
                                    if (_chosenExitRoom.isConnectionRoom) return false;
                        
                                    // Check if one of the new room's exits aligns with the exit it was spawned from
                                    foreach (Transform roomExit in _lastPlacedRoom.GetRoomExits())
                                    {
                                        if (Vector3.Distance(roomExit.position, _selectedExit.position) < 0.1f)
                                        {
                                            return true;
                                        }
                                    }
                        
                                    return false;
                                }
                        
                                private int GetExitCount()
                                {
                                    return _availableExits.Values.Sum(list => list.Count);
                                }
                            }
                        
                            public class DungeonDebugger
                            {
                                private readonly MonoBehaviour _owner;
                                private bool _delayRoomGeneration;
                                private float _delayLength;
                        
                                public DungeonDebugger(MonoBehaviour owner)
                                {
                                    _owner = owner;
                                }
                        
                                public void ApplyGenerationDelay()
                                {
                                    if (_delayRoomGeneration)
                                    {
                                        _owner.StartCoroutine(CreateDebuggableDelay());
                                    }
                                }
                        
                                private IEnumerator CreateDebuggableDelay()
                                {
                                    yield return new WaitForSeconds(_delayLength);
                                }
                            }
                        
                            [CustomEditor(typeof(DungeonGenerator))]
                            public class DungeonGeneratorEditor : Editor
                            {
                                public override void OnInspectorGUI()
                                {
                                    DrawDefaultInspector();
                                    if (GUILayout.Button("Generate Dungeon"))
                                    {
                                        // Find the dungeon Generator in the scene
                                        DungeonGenerator generator = FindFirstObjectByType<DungeonGenerator>();
                                        if (generator)
                                        {
                                            generator.InitializeDungeon();
                                        }
                                    }
                        
                                    if (GUILayout.Button("Reset Dungeon"))
                                    {
                                        DungeonGenerator generator = FindFirstObjectByType<DungeonGenerator>();
                                        if (generator)
                                        {
                                            generator.ResetDungeon();
                                        }
                                    }
                                }
                            }
                        }