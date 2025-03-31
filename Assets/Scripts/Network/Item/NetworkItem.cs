using UnityEngine;
using Mirror;

/// <summary>
/// 개별 아이템 클래스
/// 나무, 돌, 광석...
/// </summary>

public class NetworkItem : NetworkBehaviour
{
    [Header("아이템 정보")]
    [Tooltip("아이템 코드")]
    public string itemID;

    [SyncVar]
    private bool isPickedUp = false; // 아이템 획득 상태
    [SyncVar]
    private bool isAbleToPickUp = false; // 아이템 획득 가능 상태
    private Collider2D itemCollider; // 아이템 콜라이더

    void Start()
    {
        itemCollider = GetComponent<Collider2D>();
        itemCollider.enabled = false;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke("MakeItemPickable", 1.0f);
    }

    private void MakeItemPickable()
    {
        isAbleToPickUp = true;
        itemCollider.enabled = true;
    }

    public void SetItemInfo(string id)
    {
        itemID = id;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + id);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 서버에서만 처리 & 이미 획득된 아이템은 무시
        if (!isServer || isPickedUp || !isAbleToPickUp) return;

        // 플레이어와 충돌했는지 확인
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // 플레이어가 아이템 획득
            Debug.Log("플레이어가 아이템을 획득했습니다: " + player.name);

            // 아이템 획득 처리
            PlayerPickedUpItem(player);
        }
    }

    /// <summary>
    /// 서버에서 아이템 획득 처리
    /// </summary>
    /// <param name="player">아이템을 획득한 플레이어</param>
    [Server]
    private void PlayerPickedUpItem(PlayerController player)
    {
        // 획득 상태로 변경
        isPickedUp = true;

        // 아이템을 주운 플레이어에게 아이템 획득 알림 및 아이템 인벤토리에 추가
        TargetOnItemPickedUp(player.connectionToClient, itemID, 1);

        // 아이템 획득 효과를 모든 클라이언트에 알림
        RpcOnItemPickedUp();
    }

    /// <summary>
    /// 모든 클라이언트에서 아이템 획득 효과 표시 및 제거
    /// </summary>
    [ClientRpc]
    private void RpcOnItemPickedUp()
    {
        // 아이템 획득 효과 표시
        // ...

        //아이템 제거
        DestroyItem();
    }

    /// <summary>
    /// 아이템을 주운 플레이어에게 아이템 획득 알림 및 아이템 인벤토리에 추가
    /// 아이템을 획득한 클라이언트에서만 실행됨
    /// </summary>
    [TargetRpc]
    private void TargetOnItemPickedUp(NetworkConnection target, string id, int count)
    {        
        // 인벤토리 매니저를 통해 아이템 추가
        InventoryManager playerInventory = InventoryManager.Instance;
        if (playerInventory != null)
        {
            // 인벤토리에 아이템 추가
            playerInventory.AddItem(id, count);
        }
        else
        {
            Debug.LogError("인벤토리 접근 불가");
        }
    }

    // 아이템 제거
    [Server]
    private void DestroyItem()
    {
        // 서버에서 아이템 제거 (모든 클라이언트에 동기화됨)
        NetworkServer.Destroy(gameObject);
    }
}