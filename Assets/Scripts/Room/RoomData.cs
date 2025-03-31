using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    [Tooltip("Room size in unit room scale")]
    [SerializeField] private Vector2Int roomSize = Vector2Int.one;
    private bool isConnectedWithFoundation;
    public bool IsConnectedWithFoundation {  get { return isConnectedWithFoundation; } }
    private bool isConnectedWithSupport;
    public bool IsConnectedWithSupport { get { return isConnectedWithSupport; } }

    public void SetRoomData(Vector2Int setSize, bool setFoundationConnected)
    {
        roomSize = setSize;
        isConnectedWithFoundation = setFoundationConnected;
    }
}
