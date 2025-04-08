using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NonRoomType
{
    ladder = 0,
    lift = 1,
    pipe = 2
}
[Serializable]
public class NonRoom : NetworkBehaviour
{
    [Header("설치물 데이터")]
    [Tooltip("설치물 타입")]
    [SerializeField]
    private NonRoomType nonRoomType;
    public NonRoomType NonRoomType { get { return nonRoomType; } }

    [Tooltip("설치물 재료")]
    [SerializeField]
    protected RequiredItem[] requiredItems;
    public RequiredItem[] RequiredItems { get { return requiredItems; } }


    [Tooltip("설치물 위치")]
    [SerializeField]
    private Vector2Int nonRoomPosition = Vector2Int.zero;
    public Vector2Int NonRoomPosition { get { return nonRoomPosition; } }

    [Tooltip("활성화 되어있는지, 파이프의 경우에는 물이 흐르는지")]
    [SyncVar]
    [SerializeField]
    private bool isActivated = false;
    public bool IsActivated { get { return isActivated; } }

    public void Activate()
    {
        isActivated = true;
    }
    public void SetNonRoomData(Vector2Int setPosition)
    {
        nonRoomPosition = setPosition;
    }
    private void Start()
    {
        transform.localScale = BuildManager.Instance.RoomUnitSize;
    }
}