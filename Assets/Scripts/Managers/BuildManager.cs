using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : NetworkBehaviour
{
    public static BuildManager Instance { get; private set; }

    [SerializeField] private CursorController cursorController;

    [Header("Unit Room Size")]
    [SyncVar(hook = nameof(OnUnitSizeChanged))]
    [SerializeField] private Vector2 roomUnitSize = new Vector2(1, 1);
    public Vector2 RoomUnitSize { get { return roomUnitSize; } }

    [Header("Foundation")]
    [SyncVar]
    [SerializeField] private int currentFoundationLevel;
    [SerializeField] private int[] foundationMaxHeights;

    [Header("Room Coordinate")]
    [Tooltip("Data of the room constructed at the coordinates. The foundation can only be built at y: 0.")]
    [SerializeField] private SerializableDictionary<Vector2Int, Room> worldRoomData = new SerializableDictionary<Vector2Int, Room>();

    [SerializeField] private GameObject testRoomObject;
    [SerializeField] private GameObject testFoundationObject;

    [Header("설치물 데이터")]
    [Tooltip("설치물 프리팹")]
    [SerializeField] private GameObject NonRoomPrefab;
    [Tooltip("설치물 위치 데이터")]
    [SerializeField] private SerializableDictionary<Vector2Int, NonRoom> worldNonRoomData = new SerializableDictionary<Vector2Int, NonRoom>();

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
    }

    private void Start()
    {
        cursorController.SetCursorSize(roomUnitSize);
    }

    /// <summary>
    /// 방의 유닛 사이즈 변경 시 호출되는 함수
    /// </summary>
    private void OnUnitSizeChanged(Vector2 oldSize, Vector2 newSize)
    {
        cursorController.SetCursorSize(newSize);
    }

    #region Methods to check Room placement availability

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
    private bool CheckRoomExistance(Vector2Int coordinate)
    {
        if (worldRoomData.TryGetValue(coordinate, out Room room))
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
        else if (coordinate.y >= foundationMaxHeights[currentFoundationLevel])
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
        if (!worldRoomData.TryGetValue(coordinate + Vector2Int.down, out Room belowRoom))
            return false;
        else
            return belowRoom.IsConnectedWithFoundation;
    }

    /// <summary>
    /// 해당 좌표에 방이 존재해야 설치가능, 하지만 NonRoom이 있으면 설치 불가능
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public bool CanBuildNonRoom(Vector2Int coordinate)
    {
        if (worldNonRoomData.TryGetValue(coordinate, out NonRoom nonRoom))
            return false;
        else if (worldRoomData.TryGetValue(coordinate, out Room room))
            return true;
        else
            return false;
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

    #endregion

    #region Add or Build Room
    /// <summary>
    /// A method that constructs a building at the given coordinates.
    /// </summary>
    /// <param name="coordinate"></param>
    [Command(requiresAuthority = false)]
    public void CmdBuildRoom(Vector2Int coordinate)
    {
        if (CanBuildRoom(coordinate))
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
        else
        {
            Debug.Log("건물을 지을 수 없습니다.");
        }
    }
    /// <summary>
    /// 클라이언트 측에서 서버로 설치물 건설 요청 커맨드
    /// </summary>
    [Command(requiresAuthority = false)]
    public void CmdBuildNonRoom(Vector2Int coordinate, NonRoomType nonRoomType)
    {
        if (CanBuildNonRoom(coordinate))
        {
            BuildNonRoom(nonRoomType, coordinate);
        }
        else
        {
            Debug.Log("설치물을 지을 수 없습니다.");
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
                Debug.Log("일반 방 건설");
                roomObject = Instantiate(testRoomObject, FromBasisIntCoordinates(coordinate), Quaternion.identity);
                break;
            default:
                Debug.Log($"An invalid room type input has been entered.");
                return;
        }
        NetworkServer.Spawn(roomObject);
        //roomObject.transform.position = FromBasisIntCoordinates(coordinate);

        // 생성 후 방 데이터 공유
        RpcOnBuildNewRoom(coordinate, roomObject);
    }

    /// <summary>
    /// 서버에서 NonRoom을 건설하는 메서드
    /// </summary>
    [Server]
    private void BuildNonRoom(NonRoomType nonRoomType, Vector2Int coordinate)
    {
        GameObject nonRoomObject = Instantiate(NonRoomPrefab, FromBasisIntCoordinates(coordinate), Quaternion.identity);
        NetworkServer.Spawn(nonRoomObject);

        // 생성 후 방 데이터 공유
        RpcOnBuildNewNonRoom(coordinate, nonRoomObject);
    }

    /// <summary>
    /// 방 오브젝트 생성 후 모든 클라이언트에 방 관련 데이터 공유
    /// </summary>
    [ClientRpc]
    private void RpcOnBuildNewRoom(Vector2Int coordinate, GameObject roomObject)
    {
        // 추후 최적화를 위해서 풀링을 사용하는 것이 좋을 것 같음. 일단은 임시니까 깡으로 객체 생성
        AddRoomData(coordinate, roomObject.GetComponent<Room>());
    }

    [ClientRpc]
    private void RpcOnBuildNewNonRoom(Vector2Int coordinate, GameObject nonRoomObject)
    {
        // 추후 최적화를 위해서 풀링을 사용하는 것이 좋을 것 같음. 일단은 임시니까 깡으로 객체 생성
        AddNonRoomData(coordinate, nonRoomObject.GetComponent<NonRoom>());
    }

    public void AddRoomData(Vector2Int coordinate, Room room)
    {
        // 추후 지지대가 추가되면 조건을 더 추가해줘야 함
        room.SetRoomData(Vector2Int.one, true);

        if (!worldRoomData.TryAdd(coordinate, room))
        {
            Debug.LogWarning("Data already exists at the specified coordinates. Please check the room placement code again.");
        }
    }

    public void AddNonRoomData(Vector2Int coordinate, NonRoom nonRoom)
    {
        if (!worldNonRoomData.TryAdd(coordinate, nonRoom))
        {
            Debug.LogWarning("Data already exists at the specified coordinates. Please check the room placement code again.");
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
        if (worldNonRoomData.TryGetValue(coordinate, out NonRoom nonRoom))
        {
            DeleteNonRoom(coordinate);
        }
        else if (worldRoomData.TryGetValue(coordinate, out Room room))
        {
            DeleteRoom(coordinate);
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
        //DestroyRoomObject(deleteRoom.Entity.gameObject);
        // 제거 후 방 데이터 공유
        RpcOnDeleteRoom(coordinate);
    }

    [Server]
    private void DeleteNonRoom(Vector2Int coordinate)
    {
        // 제거 후 방 데이터 공유
        RpcOnDeleteNonRoom(coordinate);
    }

    /// <summary>
    /// 해당 좌표에 방이 있는지 확인 후 모든 클라이언트에서 방을 삭제
    /// </summary>
    [ClientRpc]
    private void RpcOnDeleteRoom(Vector2Int coordinate)
    {
        worldRoomData.TryGetValue(coordinate, out Room room);
        Debug.Log($"Deleting {room} of the room at coordinates {coordinate}.");
        RemoveRoomData(coordinate);
        DestroyRoomObject(room.gameObject);
    }
    /// <summary>
    /// 모든 클라이언트에서 NonRoom 삭제
    /// </summary>
    [ClientRpc]
    private void RpcOnDeleteNonRoom(Vector2Int coordinate)
    {
        worldNonRoomData.TryGetValue(coordinate, out NonRoom nonRoom);
        Debug.Log($"Deleting {nonRoom} of the non-room at coordinates {coordinate}.");
        RemoveNonRoomData(coordinate);
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
        if (worldNonRoomData.Remove(coordinate))
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
}
