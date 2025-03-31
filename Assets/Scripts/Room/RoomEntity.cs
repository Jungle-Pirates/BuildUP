using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntity : NetworkBehaviour
{
    private RoomData roomData;

    private void Start()
    {
        transform.localScale = BuildManager.Instance.RoomUnitSize;
        //BuildManager.Instance.AddRoomData(BuildManager.Instance.ToBasisIntCoordinates(transform.position), new Room(roomData, this));
    }

    private void OnDestroy()
    {
        //BuildManager.Instance.RemoveRoomData(BuildManager.Instance.ToBasisIntCoordinates(transform.position));
        
    }
}
