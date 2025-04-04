using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본 방 객체
/// </summary>
[Serializable]
public abstract class Room: NetworkBehaviour
{
    [Header("Room Data")]
    public string RoomName;
    public Sprite RoomSprite;
    public RequiredItem[] RequiredItems;
    [Tooltip("Room size in unit room scale")]
    [SerializeField] private Vector2Int roomSize = Vector2Int.one;
    private Vector2Int roomPosition = Vector2Int.zero;
    public Vector2Int RoomPosition { get { return roomPosition; } }

    private bool isConnectedWithFoundation;         // 해당하는 방이 토대와 연결되어 있는가
    public bool IsConnectedWithFoundation { get { return isConnectedWithFoundation; } }
    private bool isConnectedWithSupport;            // 해당하는 방이 지지대와 연결되어 있는가, 이 부분은 나중에 지지대를 만들 때 수정
    public bool IsConnectedWithSupport { get { return isConnectedWithSupport; } }

    public void SetRoomData(Vector2Int coordinate, Vector2Int setSize, bool setFoundationConnected)
    {
        roomPosition = coordinate;
        roomSize = setSize;
        isConnectedWithFoundation = setFoundationConnected;
    }

    protected virtual void Start()
    {
        transform.localScale = BuildManager.Instance.RoomUnitSize;
    }
}
