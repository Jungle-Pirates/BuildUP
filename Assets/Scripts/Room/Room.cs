using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Waiting,
    Completed
}

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

    [Tooltip("활성화 되어있는지, 일꾼이 일할 수 있는 상태인지")]
    [SyncVar]
    [SerializeField]
    private bool isActivated = false;

    public int maxWorkers = 1;
    private int currentWorkers = 0;
    public int CurrentWorkers => currentWorkers;
    public int priority = 0;
    public bool isAutomated = false;
    private float productionMultiplier = 0f;
    public float ProductionMultiplier => productionMultiplier;
    public TaskStatus taskStatus = TaskStatus.NotStarted;

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

    public void Activate()
    {
        isActivated = true;
    }

    private void Start()
    {
        transform.localScale = BuildManager.Instance.RoomUnitSize;
    }
    
    // 일꾼을 방에 할당
    public virtual bool AssignWorker(int count)
    {
        if (currentWorkers + count <= maxWorkers)
        {
            currentWorkers += count;
            UpdateProductionMultiplier();
            return true;
        }
        return false;
    }
    
    // 일꾼 제거
    public virtual bool RemoveWorker(int count)
    {
        if (currentWorkers - count >= 0)
        {
            currentWorkers -= count;
            UpdateProductionMultiplier();
            return true;
        }
        return false;
    }
    
    // 생산 속도 배율 업데이트
    protected virtual void UpdateProductionMultiplier()
    {
        productionMultiplier = currentWorkers / (float)maxWorkers;
    }
    
    // 상호작용 메서드
    public virtual void Interact()
    {
        // UI 열기
        // WorkerManagementUI.instance.OpenRoomUI(this);
    }
    
    public virtual void StartTask()
    {
        if (currentWorkers > 0 && isAutomated)
        {
            taskStatus = TaskStatus.InProgress;
            // 작업 처리 로직
        }
        else if (currentWorkers == 0 && isAutomated)
        {
            taskStatus = TaskStatus.Waiting;
        }
    }
    
    // 작업 완료
    public virtual void CompleteTask()
    {
        taskStatus = TaskStatus.Completed;
        // 다음 작업 설정 또는 작업 종료
    }
}
