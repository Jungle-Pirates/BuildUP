using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Room
{
    private RoomData data;
    public RoomData Data {  get { return data; } }
    private RoomEntity entity;
    public RoomEntity Entity { get { return entity; } }

    public Room(RoomData data, RoomEntity entity)
    {
        this.data = data;
        this.entity = entity;
    }

    public void SetRoomData(RoomData rdata)
    {
        data = rdata;
    }
}
