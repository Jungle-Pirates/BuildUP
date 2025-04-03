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
public class NonRoom : MonoBehaviour
{
    [Header("설치물 데이터")]
    [Tooltip("설치물 타입")]
    [SerializeField]
    private NonRoomType nonRoomType;

    [Tooltip("설치물 위치")]
    [SerializeField]
    private Vector2Int nonRoomPosition = Vector2Int.zero;

    public void SetNonRoomData(Vector2Int setPosition)
    {
        nonRoomPosition = setPosition;
    }
}