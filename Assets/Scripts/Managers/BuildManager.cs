using Mirror;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    토대, 빈방, 제작방, 제련방, 농장, 취사장, 주거지, 빗물저장소, 물레, 방앗간
}
public class BuildManager : NetworkBehaviour
{
    public static BuildManager Instance { get; private set; }

    [SerializeField] private CursorController cursorController;

    [Header("Unit Room Size")]
    [SyncVar(hook = nameof(OnUnitSizeChanged))]
    [SerializeField] private Vector2 roomUnitSize = new Vector2(1, 1);
    public Vector2 RoomUnitSize { get { return roomUnitSize; } }

    [Header("Room Coordinate")]
    [Tooltip("Data of the room constructed at the coordinates. The foundation can only be built at y: 0.")]
    [SerializeField] private SyncDictionary<Vector2Int, GameObject> worldRoomData = new SyncDictionary<Vector2Int, GameObject>();
    public SyncDictionary<Vector2Int, GameObject> WorldRoomData => worldRoomData;

    [SerializeField] private GameObject testFoundationObject;
    [SerializeField] private GameObject emptyRoom;

    private bool _isBuildMode = false;
    public bool IsBuildMode { get { return _isBuildMode; } }
    private bool _isPlaceMode = false;
    public bool IsPlaceMode { get { return _isPlaceMode; } }
    private bool _isDestructionMode = false;
    public bool IsDestructionMode { get { return _isDestructionMode; } }

    private int _prefabIndex = 0;
    public int PrefabIndex { get { return _prefabIndex; } }
    private Room _selectRoom;
    public Room SelectRoom { get { return _selectRoom; } }

    [Header("UI")]
    [SerializeField] private GameObject buildUI;

    [Header("설치물 데이터")]
    [Tooltip("설치물 프리팹")]
    [SerializeField] private GameObject LadderPrefab;
    [SerializeField] private GameObject PipePrefab;
    [Tooltip("사다리 위치 데이터")]
    [SerializeField] private SyncDictionary<Vector2Int, GameObject> worldLadderData = new SyncDictionary<Vector2Int, GameObject>();
    [Tooltip("수로 위치 데이터")]
    [SerializeField] private SyncDictionary<Vector2Int, GameObject> worldPipeData = new SyncDictionary<Vector2Int, GameObject>();

    #region Monobehavior
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cursorController == null)
        {
            cursorController = FindFirstObjectByType<CursorController>();
        }
        if (buildUI == null)
        {
            buildUI = FindAnyObjectByType<BuildUI>().gameObject;
        }
    }

    private void Start()
    {
        cursorController.SetCursorSize(roomUnitSize);
        SetSelectedRoom(BuildUpgradeManager.Instance.Rooms[0].GetComponent<Room>(), _prefabIndex);
    }

    private void Update()
    {
        //임시로 'B'를 모드 전환 되도록
        if (Input.GetKeyDown(KeyCode.B))
        {
            _isDestructionMode = false;

            if (_isBuildMode)
            {
                _isBuildMode = false;
                _isPlaceMode = true;
            }
            else if (_isPlaceMode)
            {
                _isPlaceMode = false;
                _isBuildMode = false;
            }
            else
            {
                _isBuildMode = true;
                _isPlaceMode = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            _isPlaceMode = false;
            _isBuildMode = false;

            if (_isDestructionMode)
            {
                _isDestructionMode = false;
            }
            else
            {
                _isDestructionMode = true;
            }
        }

        if (buildUI != null)
        {
            if (_isBuildMode)
            {
                buildUI.SetActive(true);
            }
            else
            {
                buildUI.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("There is no Build UI in the current scene.");
        }
    }
    #endregion

    /// <summary>
    /// 방의 유닛 사이즈 변경 시 호출되는 함수
    /// </summary>
    private void OnUnitSizeChanged(Vector2 oldSize, Vector2 newSize)
    {
        cursorController.SetCursorSize(newSize);
    }

    #region Check Room placement availability

    /// <summary>
    /// A method that returns whether a building can be placed at the given coordinates.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public bool CanBuildRoom(Vector2Int coordinate)
    {
        if (CheckRoomExistance(coordinate) || !CanBuildAtHeight(coordinate))
            return false;

        return (coordinate.y == 0 || HasRoomConnectedToFoundationBelow(coordinate));
    }

    /// <summary>
    /// A method that returns whether a room exists at the given coordinates.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public bool CheckRoomExistance(Vector2Int coordinate)
    {
        if (worldRoomData.TryGetValue(coordinate, out GameObject room))
            return true;
        else
            return false;
    }

    /// <summary>
    /// A method that returns whether the coordinates are within a valid height range for building.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    private bool CanBuildAtHeight(Vector2Int coordinate)
    {
        if (coordinate.y < 0)
        {
            //Debug.Log("Rooms cannot be constructed at coordinates lower than 0.");
            return false;
        }
        else if (coordinate.y >= FoundationManager.Instance.FoundationMaxHeights[FoundationManager.Instance.CurrentFoundationLevel])
        {
            //Debug.Log("Rooms cannot be constructed at heights higher than the current maximum building height.");
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Method to check whether there is a room connected to the foundation below
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    private bool HasRoomConnectedToFoundationBelow(Vector2Int coordinate)
    {
        if (!worldRoomData.TryGetValue(coordinate + Vector2Int.down, out GameObject belowRoom))
            return false;
        else
            return belowRoom.GetComponent<Room>().IsConnectedWithFoundation;
    }

    /// <summary>
    /// 해당 좌표에 방이 존재해야 설치가능, 하지만 Ladder 있으면 설치 불가능
    /// </summary>
    public bool CanBuildLadder(Vector2Int coordinate)
    {
        if (worldLadderData.TryGetValue(coordinate, out GameObject nonRoom))
            return false;
        else if (worldRoomData.TryGetValue(coordinate, out GameObject room))
            return true;
        else
            return false;
    }
    /// <summary>
    /// 해당 좌표에 방이 존재해야 설치가능, 하지만 Pipe 있으면 설치 불가능
    /// </summary>
    public bool CanBuildPipe(Vector2Int coordinate)
    {
        if (worldPipeData.TryGetValue(coordinate, out GameObject nonRoom))
            return false;
        else if (worldRoomData.TryGetValue(coordinate, out GameObject room))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 해당하는 방을 설치할 수 있는 자원을 가지고 있는가를 반환
    /// </summary>
    /// <returns>true: 자원 충분, false: 자원 부족</returns>
    public bool HasResourceToBuild(Room room)
    {
        foreach (RequiredItem requiredItem in room.RequiredItems)
        {
            if (!InventoryManager.Instance.HasItemAmount(requiredItem.itemID, requiredItem.amount))
                return false;
        }
        return true;
    }

    public bool HasResourceToBuild(NonRoom nonRoom)
    {
        foreach (RequiredItem requiredItem in nonRoom.RequiredItems)
        {
            if (!InventoryManager.Instance.HasItemAmount(requiredItem.itemID, requiredItem.amount))
                return false;
        }
        return true;
    }

    #endregion

    #region 방 인접 정보

    /// <summary>
    /// 인접하고 있는 방의 정보를 가져오는 메서드
    /// </summary>
    // public void GetConnectedRoomData(Room room, out List<Room> connectedRoom)
    // {
    //     connectedRoom = new List<Room>();
    //     Vector2Int coordinate = 

    //     // 방의 좌표를 기준으로 인접한 방의 좌표를 가져옴
    //     Vector2Int[] adjacentCoordinates = new Vector2Int[4]
    //     {
    //         coordinate + Vector2Int.up,
    //         coordinate + Vector2Int.down,
    //         coordinate + Vector2Int.left,
    //         coordinate + Vector2Int.right
    //     };

    //     // 인접한 방이 존재하는지 확인 후 리스트에 추가
    //     foreach (Vector2Int adjacentCoordinate in adjacentCoordinates)
    //     {
    //         if (worldRoomData.TryGetValue(adjacentCoordinate, out Room adjacentRoom))
    //         {
    //             connectedRoom.Add(adjacentRoom);
    //         }
    //     }
    // }

    /// <summary>
    /// 인접한 모든 설치물을 활성화 하는 코드 - BFS를 이어진 모든 PIPE를 활성화 시킴
    /// 나중에 파이프가 설치될때, 혹은 물탱크가 설치될때 이 메서드를 호출하면 됨
    /// </summary>
    /// <param name="waterTankCoordinate">물탱크 혹은 활성화된 파이프 위치(아마 대부분 물탱크)</param>
    [Server]
    public void ActivatePipe(Vector2Int waterTankCoordinate)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(waterTankCoordinate);
        visited.Add(waterTankCoordinate);

        while (queue.Count > 0)
        {
            Vector2Int currentCoordinate = queue.Dequeue();
            if (worldPipeData.TryGetValue(currentCoordinate, out GameObject pipe))
            {
                pipe.GetComponent<NonRoom>().Activate();
                //해당 위치에 물레가 존재하면 인접한 방을 활성화
                var room = worldRoomData.GetValueOrDefault(currentCoordinate, null);
                Room roomComponent = room?.GetComponent<Room>();
                if (room != null && roomComponent.roomType == RoomType.물레)
                {
                    roomComponent.Activate();
                    ActivatePower(currentCoordinate);
                }
                // 인접한 좌표를 큐에 추가
                Vector2Int[] adjacentCoordinates = new Vector2Int[4]
                {
                    currentCoordinate + Vector2Int.up,
                    currentCoordinate + Vector2Int.down,
                    currentCoordinate + Vector2Int.left,
                    currentCoordinate + Vector2Int.right
                };

                foreach (Vector2Int adjacentCoordinate in adjacentCoordinates)
                {
                    if (!visited.Contains(adjacentCoordinate) && worldPipeData.ContainsKey(adjacentCoordinate))
                    {
                        queue.Enqueue(adjacentCoordinate);
                        visited.Add(adjacentCoordinate);
                    }
                }
            }
        }
    }
    [Server]
    public void ActivatePower(Vector2Int waterMillCoordinate)
    {
        Vector2Int[] adjacentCoordinates = new Vector2Int[4]
       {
            waterMillCoordinate + Vector2Int.up,
            waterMillCoordinate + Vector2Int.down,
            waterMillCoordinate + Vector2Int.left,
            waterMillCoordinate + Vector2Int.right
       };
        foreach (Vector2Int adjacentCoordinate in adjacentCoordinates)
        {
            if (worldRoomData.TryGetValue(adjacentCoordinate, out GameObject room))
            {
                room.GetComponent<Room>().Activate();
            }
        }
    }

    #endregion

    #region Add or Build Room

    /// <summary>
    /// 방 건설 관련 동작 시 호출 메서드.
    /// - 빈 방 건설을 선택했으면 새 건물 건설 실행
    /// - 빈 방이 아닌 건물 건설을 선택했으면 BuildUpgradeManager로 업그레이드 메서드 호출
    /// </summary>
    /// <param name="coordinate"></param>
    public void BuildOrUpgradeRoom(Vector2Int coordinate)
    {
        if (_selectRoom.IsEmptyRoom)
        {
            CheckAndBuildRoom(coordinate);
        }
        else
        {
            BuildUpgradeManager.Instance.CheckAndUpgradeRoom(coordinate);
        }
    }

    /// <summary>
    /// A method that constructs a building at the given coordinates.
    /// </summary>
    /// <param name="coordinate"></param>
    public void CheckAndBuildRoom(Vector2Int coordinate)
    {
        // 새 방 생성
        if (!CanBuildRoom(coordinate))
        {
            Debug.Log("<color=red>해당 위치에 건물을 지을 수 없습니다.</color>");
        }
        else if (!HasResourceToBuild(_selectRoom))
        {
            Debug.Log("<color=red>해당 방을 짓기에 자원이 부족합니다.</color>");
        }
        else
        {
            UseRequiredItem(_selectRoom);
            CmdBuildRoom(coordinate);
        }
    }

    public void CheckAndBuildNonRoom(Vector2Int coordinate, NonRoomType nonRoomType)
    {
        if (nonRoomType == NonRoomType.pipe)
        {
            if (!CanBuildPipe(coordinate))
            {
                Debug.Log("<color=red>해당 위치에 설치물을 지을 수 없습니다.</color>");
            }
            else if (!HasResourceToBuild(PipePrefab.GetComponent<NonRoom>()))
            {
                Debug.Log("<color=red>해당 방을 짓기에 자원이 부족합니다.</color>");
            }
            else
            {
                foreach (RequiredItem item in PipePrefab.GetComponent<NonRoom>().RequiredItems)
                {
                    InventoryManager.Instance.RemoveItem(item.itemID, item.amount);
                }
                CmdBuildNonRoom(coordinate, nonRoomType);
            }
        }
        else if (nonRoomType == NonRoomType.ladder)
        {
            if (!CanBuildLadder(coordinate))
            {
                Debug.Log("<color=red>해당 위치에 설치물을 지을 수 없습니다.</color>");
            }
            else if (!HasResourceToBuild(LadderPrefab.GetComponent<NonRoom>()))
            {
                Debug.Log("<color=red>해당 방을 짓기에 자원이 부족합니다.</color>");
            }
            else
            {
                foreach (RequiredItem item in LadderPrefab.GetComponent<NonRoom>().RequiredItems)
                {
                    InventoryManager.Instance.RemoveItem(item.itemID, item.amount);
                }
                CmdBuildNonRoom(coordinate, nonRoomType);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdBuildRoom(Vector2Int coordinate)
    {
        if (coordinate.y == 0)
        {
            BuildRoom(0, coordinate);
        }
        else
        {
            BuildRoom(1, coordinate);
        }
    }

    /// <summary>
    /// 클라이언트 측에서 서버로 설치물 건설 요청 커맨드
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdBuildNonRoom(Vector2Int coordinate, NonRoomType nonRoomType)
    {
        if (nonRoomType == NonRoomType.pipe)
        {
            BuildNonRoom(nonRoomType, coordinate);
        }
        else if (nonRoomType == NonRoomType.ladder)
        {
            BuildNonRoom(nonRoomType, coordinate);
        }
        else
        {
            Debug.LogError("설치물 타입이 잘못되었습니다.");
        }
    }

    /// <summary>
    /// 플레이어가 커맨드로 방 건설을 요청하면 서버에서 방을 건설
    /// </summary>
    /// <param name="roomType">일단은 임시로 int 값에 의해서 방 종류 결정(0=빈방, 1=토대)</param>
    [Server]
    private void BuildRoom(int roomType, Vector2Int coordinate)
    {
        // 방 코드를 통해서 방 프리팹 선택 후 방 오브젝트 생성
        GameObject roomObject;
        switch (roomType)
        {
            case 0:
                Debug.Log("토대 건설");
                roomObject = Instantiate(testFoundationObject, FromBasisIntCoordinates(coordinate), Quaternion.identity);
                break;
            case 1:
                Debug.Log("빈 방 건설");
                roomObject = Instantiate(emptyRoom, FromBasisIntCoordinates(coordinate), Quaternion.identity);
                break;
            default:
                Debug.Log($"An invalid room type input has been entered.");
                return;
        }
        NetworkServer.Spawn(roomObject);

        // 생성 후 방 데이터 공유
        RpcOnBuildNewRoom(coordinate, roomObject);
    }

    /// <summary>
    /// 서버에서 NonRoom을 건설하는 메서드
    /// </summary>
    [Server]
    private void BuildNonRoom(NonRoomType nonRoomType, Vector2Int coordinate)
    {
        GameObject nonRoomObject = null;
        if (nonRoomType == NonRoomType.ladder)
        {
            Debug.Log("사다리 건설");
            // 사다리 프리팹을 통해서 NonRoom 오브젝트 생성
            nonRoomObject = Instantiate(LadderPrefab, FromBasisIntCoordinates(coordinate), Quaternion.identity);
            NetworkServer.Spawn(nonRoomObject);
        }
        else if (nonRoomType == NonRoomType.pipe)
        {
            Debug.Log("파이프 건설");
            // 파이프 프리팹을 통해서 NonRoom 오브젝트 생성
            nonRoomObject = Instantiate(PipePrefab, FromBasisIntCoordinates(coordinate), Quaternion.identity);
            NetworkServer.Spawn(nonRoomObject);
        }
        else
        {
            Debug.LogError("NonRoomType이 잘못되었습니다.");
            return;
        }
        // 생성 후 방 데이터 공유
        RpcOnBuildNewNonRoom(coordinate, nonRoomObject);
    }

    /// <summary>
    /// 방 오브젝트 생성 후 모든 클라이언트에 방 관련 데이터 공유
    /// </summary>
    private void RpcOnBuildNewRoom(Vector2Int coordinate, GameObject roomObject)
    {
        // 추후 최적화를 위해서 풀링을 사용하는 것이 좋을 것 같음. 일단은 임시니까 깡으로 객체 생성
        AddRoomData(coordinate, roomObject.GetComponent<Room>());
    }

    private void RpcOnBuildNewNonRoom(Vector2Int coordinate, GameObject nonRoomObject)
    {
        if (nonRoomObject.GetComponent<NonRoom>().NonRoomType == NonRoomType.ladder)
        {
            // 사다리일 경우에만 방 데이터 추가
            AddLadderData(coordinate, nonRoomObject.GetComponent<NonRoom>());
            return;
        }
        if (nonRoomObject.GetComponent<NonRoom>().NonRoomType == NonRoomType.pipe)
        {
            AddPipeData(coordinate, nonRoomObject.GetComponent<NonRoom>());
            return;
        }
        Debug.LogError("NonRoomType이 잘못되었습니다.");
        return;
    }

    public void AddRoomData(Vector2Int coordinate, Room room)
    {
        // 추후 지지대가 추가되면 조건을 더 추가해줘야 함
        room.SetRoomData(coordinate, Vector2Int.one, true);

        if (!worldRoomData.TryAdd(coordinate, room.gameObject))
        {
            Debug.LogWarning("Data already exists at the specified coordinates. Please check the room placement code again.");
            return;
        }
        // 주변에 물레가 있을경우 활성화
        Vector2Int[] adjacentCoordinates = new Vector2Int[4]
        {
            coordinate + Vector2Int.up,
            coordinate + Vector2Int.down,
            coordinate + Vector2Int.left,
            coordinate + Vector2Int.right
        };
        foreach (Vector2Int adjacentCoordinate in adjacentCoordinates)
        {
            if (worldRoomData.TryGetValue(adjacentCoordinate, out GameObject roomObj))
            {
                if (roomObj.GetComponent<Room>().roomType == RoomType.물레)
                {
                    room.Activate();
                }
            }
        }
    }

    public void AddLadderData(Vector2Int coordinate, NonRoom nonRoom)
    {
        if (!worldLadderData.TryAdd(coordinate, nonRoom.gameObject))
        {
            Debug.LogWarning("Data already exists at the specified coordinates. Please check the room placement code again.");
            return;
        }
    }
    public void AddPipeData(Vector2Int coordinate, NonRoom nonRoom)
    {
        if (!worldPipeData.TryAdd(coordinate, nonRoom.gameObject))
        {
            Debug.LogWarning("Data already exists at the specified coordinates. Please check the room placement code again.");
            return;
        }
        foreach (var room in worldRoomData)
        {
            if (room.Value.GetComponent<Room>().roomType == RoomType.빗물저장소)
            {
                ActivatePipe(room.Key);
            }
        }
    }

    /// <summary>
    /// 해당하는 방을 지을 때 소모되는 자원을 차감하는 함수
    /// </summary>
    /// <param name="room"></param>
    public void UseRequiredItem(Room room)
    {
        foreach (var required in room.RequiredItems)
        {
            InventoryManager.Instance.RemoveItem(required.itemID, required.amount);
        }
    }

    #endregion

    #region Delete Room

    /// <summary>
    /// 클라이언트 측에서 서버로 건물 삭제 요청 커맨드
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdDeleteRoom(Vector2Int coordinate)
    {
        if (worldLadderData.TryGetValue(coordinate, out GameObject nonRoom))
        {
            DeleteLadder(coordinate);
        }
        else if (worldPipeData.TryGetValue(coordinate, out GameObject nonRoom2))
        {
            DeletePipe(coordinate);
        }
        else if (worldRoomData.TryGetValue(coordinate, out GameObject roomObj))
        {
            Room room = roomObj.GetComponent<Room>();
            // 빈 방이거나 토대면 방 오브젝트를 제거
            if (room.IsEmptyRoom || room.IsFoundationRoom)
            {
                DeleteRoom(coordinate);
            }
            // 아니라면 빈 방으로 다운그레이드
            else
            {
                BuildUpgradeManager.Instance.RoomDowngrade(coordinate);
            }
        }
        else
        {
            Debug.LogWarning($"There is no room at the specified coordinates.");
            return;
        }
    }

    /// <summary>
    /// 방 제거 커맨드입력 시 클라이언트에 방 제거 명령을 RPC로 뿌림
    /// </summary>
    [Server]
    private void DeleteRoom(Vector2Int coordinate)
    {
        // 제거 후 방 데이터 공유
        RpcOnDeleteRoom(coordinate);
    }

    [Server]
    private void DeleteLadder(Vector2Int coordinate)
    {
        // 제거 후 방 데이터 공유
        RpcOnDeleteLadder(coordinate);
    }
    [Server]
    private void DeletePipe(Vector2Int coordinate)
    {
        // 제거 후 방 데이터 공유
        RpcOnDeletePipe(coordinate);
    }

    /// <summary>
    /// 해당 좌표에 방이 있는지 확인 후 모든 클라이언트에서 방을 삭제
    /// </summary>
    private void RpcOnDeleteRoom(Vector2Int coordinate)
    {
        worldRoomData.TryGetValue(coordinate, out GameObject room);
        Debug.Log($"Deleting {room} of the room at coordinates {coordinate}.");
        RemoveRoomData(coordinate);
        DestroyRoomObject(room.gameObject);
    }
    /// <summary>
    /// 모든 클라이언트에서 NonRoom 삭제
    /// </summary>
    private void RpcOnDeleteLadder(Vector2Int coordinate)
    {
        worldLadderData.TryGetValue(coordinate, out GameObject nonRoom);
        Debug.Log($"Deleting {nonRoom} of the non-room at coordinates {coordinate}.");
        RemoveNonRoomData(coordinate);
        DestroyRoomObject(nonRoom.gameObject);
    }
    private void RpcOnDeletePipe(Vector2Int coordinate)
    {
        worldPipeData.TryGetValue(coordinate, out GameObject nonRoom);
        Debug.Log($"Deleting {nonRoom} of the non-room at coordinates {coordinate}.");
        RemovePipeData(coordinate);
        DestroyRoomObject(nonRoom.gameObject);
    }

    private bool RemoveRoomData(Vector2Int coordinate)
    {
        if (worldRoomData.Remove(coordinate))
        {
            return true;
        }
        else
        {
            Debug.LogWarning("There is no room at the specified coordinates, so it cannot be deleted.");
            return false;
        }
    }

    private bool RemoveNonRoomData(Vector2Int coordinate)
    {
        if (worldLadderData.Remove(coordinate))
        {
            return true;
        }
        else
        {
            Debug.LogWarning("There is no non-room at the specified coordinates, so it cannot be deleted.");
            return false;
        }
    }

    private bool RemovePipeData(Vector2Int coordinate)
    {
        if (worldPipeData.Remove(coordinate))
        {
            return true;
        }
        else
        {
            Debug.LogWarning("There is no non-room at the specified coordinates, so it cannot be deleted.");
            return false;
        }
    }

    [Server]
    private void DestroyRoomObject(GameObject room)
    {
        if (room == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>파괴할 방이 null입니다.");
            return;
        }
        NetworkServer.Destroy(room);
    }

    #endregion

    /// <summary>
    /// 기존의 방 데이터를 다른 방으로 교체
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="replaceRoom"></param>
    public bool ReplaceRoomData(Vector2Int coordinate, Room replaceRoom)
    {
        // 기존의 방 데이터 제거
        if (worldRoomData.Remove(coordinate))
        {
            // 교체한 방 데이터 추가
            replaceRoom.SetRoomData(coordinate, Vector2Int.one, true);

            if (!worldRoomData.TryAdd(coordinate, replaceRoom.gameObject))
            {
                Debug.LogWarning("<color=red>[에러 발생]</color>데이터를 추가하고자 하는 좌표에 이미 데이터가 있습니다.");
                return false;
            }
            if (replaceRoom.roomType == RoomType.빗물저장소)
            {
                // 방이 빗물 저장소일 경우 인접한 파이프를 활성화
                ActivatePipe(coordinate);
            }
            // 방이 물레일 경우
            else if (replaceRoom.roomType == RoomType.물레)
            {
                // 해당위치에 활성화된 파이프가 존재하고
                if (worldPipeData.TryGetValue(coordinate, out GameObject pipe))
                {
                    //해당 파이프가 활성화 되어있다면
                    if (pipe.GetComponent<NonRoom>().IsActivated)
                    {
                        // 인접한 방을 활성화
                        ActivatePower(coordinate);
                    }
                }
            }
            return true;
        }
        else
        {
            Debug.LogWarning("<color=red>[에러 발생]</color>제거하고자 하는 좌표의 데이터가 존재하지 않습니다.");
            return false;
        }
    }

    #region Unit Room Size Basis Coordinate Conversion Method

    /// <summary>
    /// Returns integer coordinates transformed based on the basis (room unit size).
    /// </summary>
    /// <param name="vec">The vector to be transformed</param>
    /// <param name="unitSize">room unit size</param>
    /// <returns></returns>
    public Vector2Int ToBasisIntCoordinates(Vector2 vec)
    {
        return new Vector2Int(
            (int)((vec.x < 0 ? vec.x - roomUnitSize.x : vec.x) / roomUnitSize.x),
            (int)((vec.y < 0 ? vec.y - roomUnitSize.y : vec.y) / roomUnitSize.y));
    }

    /// <summary>
    /// Converts integer coordinates (integer multiples of the basis) back to their original position in the coordinate system.
    /// </summary>
    /// <param name="vecInt">integer coordinates</param>
    /// <param name="unitSize">room unit size</param>
    /// <returns></returns>
    public Vector2 FromBasisIntCoordinates(Vector2Int vecInt)
    {
        return new Vector2(vecInt.x * roomUnitSize.x, vecInt.y * roomUnitSize.y);
    }

    #endregion

    /// <summary>
    /// 플레이어가 설치하고자 하는 방
    /// </summary>
    /// <param name="room"></param>
    public void SetSelectedRoom(Room room, int index)
    {
        _selectRoom = room;
        _prefabIndex = index;
    }

    /// <summary>
    /// 입력한 좌표에 Room을 반환하는 메서드
    /// - 해당 좌표에 방이 존재하지 않으면 null을 반환
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public Room GetRoomWithCoordinate(Vector2Int coordinate)
    {
        if (CheckRoomExistance(coordinate))
        {
            worldRoomData.TryGetValue(coordinate, out GameObject roomObj);
            return roomObj.GetComponent<Room>();
        }
        else
        {
            return null;
        }
    }
}
