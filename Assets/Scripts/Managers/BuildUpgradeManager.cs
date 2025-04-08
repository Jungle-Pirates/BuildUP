using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUpgradeManager : NetworkBehaviour
{
    public static BuildUpgradeManager Instance { get; private set; }

    public GameObject[] Rooms;
    public GameObject EmptyRoom;

    private Room _selectRoom => BuildManager.Instance.SelectRoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 해당하는 좌표에 방을 업그레이드하는 커멘드 메서드
    /// </summary>
    /// <param name="coordinate"></param>
    public void CheckAndUpgradeRoom(Vector2Int coordinate)
    {
        if (!CheckUpgradableRoom(coordinate))
        {
            Debug.Log("<color=red>업그레이드를 위해서는 빈 방을 선택해야 합니다.</color>");
        }
        else if (!BuildManager.Instance.HasResourceToBuild(_selectRoom))
        {
            Debug.Log("<color=red>해당 방으로 업그레이드할 자원이 부족합니다.</color>");
        }
        else
        {
            // 업그레이드 가능 조건 충족 시,
            // 자원 소모 후 방 업그레이드
            BuildManager.Instance.UseRequiredItem(_selectRoom);
            CmdUpgradeRoom(coordinate, BuildManager.Instance.PrefabIndex);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdUpgradeRoom(Vector2Int coordinate, int roomPrefabIndex)
    {
        ReplaceRoom(coordinate, roomPrefabIndex);
    }

    /// <summary>
    /// 입력한 좌표에 방을 업그레이드할 수 있는 빈 방인지 여부를 반환하는 메서드
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns>좌표에 방이 없거나 빈 방이 아니면 false 반환, 빈방이면 true 반환</returns>
    private bool CheckUpgradableRoom(Vector2Int coordinate)
    {
        Room room = BuildManager.Instance.GetRoomWithCoordinate(coordinate);
        if (room != null)
        {
            return room.IsEmptyRoom;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 커맨드를 통해서 방 업그레이드 요청이 들어오면, 서버에서 방 오브젝트를 바꿔준 다음 방 데이터를 연동하는 메서드
    /// </summary>
    /// <param name="coordinate">업그레이드할 좌표</param>
    [Server]
    private void ReplaceRoom(Vector2Int coordinate, int roomPrefabIndex)
    {
        // 새 방 오브젝트를 생성
        GameObject newRoomObj = Instantiate(Rooms[roomPrefabIndex], BuildManager.Instance.FromBasisIntCoordinates(coordinate), Quaternion.identity);
        NetworkServer.Spawn(newRoomObj);

        // 기존 방 데이터 불러오기
        Room oldRoom = BuildManager.Instance.GetRoomWithCoordinate(coordinate);

        // 새로 만들어진 방으로 데이터 교체
        if (BuildManager.Instance.ReplaceRoomData(coordinate, newRoomObj.GetComponent<Room>()))
        {
            // 새 방으로 데이터 교체 후 기존 방 오브젝트 제거
            NetworkServer.Destroy(oldRoom.gameObject);
        }
    }

    [Server]
    public void RoomDowngrade(Vector2Int coordinate)
    {
        ReplaceRoom(coordinate, 0);
    }
}
