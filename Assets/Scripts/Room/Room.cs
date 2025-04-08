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
public abstract class Room : NetworkBehaviour
{
    [Header("Room Data")]
    [Tooltip("방 타입")]
    public RoomType roomType = RoomType.빈방;
    public string RoomName;
    public Sprite RoomSprite;
    public RequiredItem[] RequiredItems;
    [Tooltip("Room size in unit room scale")]
    [SerializeField] private Vector2Int roomSize = Vector2Int.one;
    private Vector2Int roomPosition = Vector2Int.zero;
    public Vector2Int RoomPosition { get { return roomPosition; } }

    [Tooltip("활성화 되어있는지, 일꾼이 일할 수 있는 상태인지")]
    [SyncVar(hook = nameof(OnIsActivatedChanged))]
    [SerializeField]
    protected bool isActivated = false;
    public bool IsActivated { get { return isActivated; } }
    [SyncVar(hook = nameof(OnIsOccupiedChanged))]
    [SerializeField]
    private bool isOccupied = false; // 다른 클라이언트가 방을 사용중인지 확인하기 위한 변수
    public bool IsOccupied { get { return isOccupied; } }
    public bool isOccupiedByMe = false; // 내가 방을 사용중인지 확인하기 위한 변수

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

    [SerializeField] private bool isEmptyRoom = false;
    public bool IsEmptyRoom { get { return isEmptyRoom; } }
    [SerializeField] private bool isFoundationRoom = false;
    public bool IsFoundationRoom { get { return isFoundationRoom; } }

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

    /// <summary>
    /// 서버에게 방이 활성화 되었음을 알림
    /// </summary>
    [Command(requiresAuthority = false)] // 아무 클라이언트나 호출 가능
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
    /// <summary>
    /// 방이 활성화 되면 상태를 변경해줌
    /// </summary>
    public void OnIsOccupiedChanged(bool oldValue, bool newValue)
    {
        isOccupied = newValue;
    }

    public void OnIsActivatedChanged(bool oldValue, bool newValue)
    {
        isActivated = newValue;
        if (isActivated)
        {
            //CraftingRoom이면 UI에서 자동화 UI on
            if (this is CraftingRoom)
            {
                GetComponent<CraftingRoom>().ShowAutoCrafting(true);
            }
        }
        else
        {
            //CraftingRoom이면 UI에서 자동화 UI off
            if (this is CraftingRoom)
            {
                GetComponent<CraftingRoom>().ShowAutoCrafting(false);
            }
        }
    }

    protected virtual void Start()
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
            if (currentWorkers > 0)
                isActivated = true;
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
            if (currentWorkers == 0)
                isActivated = false;
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
    /// <summary>
    /// 방 UI를 여는 함수, 각 방마다 다르게 구현해야 함
    /// </summary>
    public virtual void OpenRoomUI()
    {
        Debug.Log("방 UI를 여는 함수입니다.");
    }
    /// <summary>
    /// 방 UI를 닫는 함수, 각 방마다 다르게 구현해야 함
    /// </summary>
    public virtual void CloseRoomUI()
    {
        Debug.Log("방 UI를 닫는 함수입니다.");
    }
}
