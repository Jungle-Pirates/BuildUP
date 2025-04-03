using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room: NetworkBehaviour
{
    [Header("Room Data")]
    public string RoomName;
    public Sprite RoomSprite;
    public RequiredItem[] RequiredItems;
    [Tooltip("Room size in unit room scale")]
    [SerializeField] private Vector2Int roomSize = Vector2Int.one;

    private bool isConnectedWithFoundation;         // 해당하는 방이 토대와 연결되어 있는가
    public bool IsConnectedWithFoundation { get { return isConnectedWithFoundation; } }
    private bool isConnectedWithSupport;            // 해당하는 방이 지지대와 연결되어 있는가, 이 부분은 나중에 지지대를 만들 때 수정
    public bool IsConnectedWithSupport { get { return isConnectedWithSupport; } }

    public void SetRoomData(Vector2Int setSize, bool setFoundationConnected)
    {
        roomSize = setSize;
        isConnectedWithFoundation = setFoundationConnected;
    }

    private void Start()
    {
        transform.localScale = BuildManager.Instance.RoomUnitSize;
    }


}
