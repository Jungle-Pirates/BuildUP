using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// 자원 클래스 (나무, 바위...)
/// </summary>
public class NetworkResource : NetworkBehaviour
{
    [Header("자원 설정")]
    [Tooltip("드롭할 아이템 프리팹")]
    [SerializeField]
    private GameObject dropItemPrefab;
    [Tooltip("드롭할 아이템 정보")]
    [SerializeField]
    private List<DropItemSet> dropItemSets = new List<DropItemSet>();

    [Tooltip("자원 체력")]
    [SerializeField]
    private int resourceHealth = 5;
    [SerializeField]
    private Image healthBar;    //체력 게이지
    private GameObject healthBarBG; //체력 게이지 배경

    [SerializeField]
    [Range(0, 5)]
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int currentHealth; // 현재 체력

    void Start()
    {
        healthBar.fillAmount = 1;
        healthBarBG = healthBar.transform.parent.gameObject;
        healthBarBG.SetActive(false);
    }
    /// <summary>
    /// 시작할 때 체력 초기화
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = resourceHealth;
    }

    /// <summary>
    /// 클라이언트가 자원을 때렸을 때 호출 (플레이어 스크립트에서 호출)
    /// </summary>
    [Command(requiresAuthority = false)] // 아무 클라이언트나 호출 가능
    public void CmdHitResource()
    {
        // 이미 파괴된 자원이면 무시
        if (currentHealth <= 0)
        {
            return;
        }

        // 체력 감소
        currentHealth--;

        // 파괴됐는지 확인
        if (currentHealth <= 0)
        {
            // 아이템 드롭
            DropItem();

            // 자원 오브젝트 파괴
            NetworkServer.Destroy(gameObject);
        }
    }

    /// <summary>
    /// 체력 변경 시 호출되는 Hook 함수
    /// </summary>
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // 체력에 따른 시각적 변화 적용
        UpdateHealthState(newHealth);
    }

    /// <summary>
    /// 체력바 감소
    /// </summary>
    private void UpdateHealthState(int health)
    {
        healthBarBG?.SetActive(health < resourceHealth);
        healthBar.fillAmount = (float)health / resourceHealth;
    }

    /// <summary>
    /// 서버에서 아이템 드롭
    /// </summary>
    [Server]
    private void DropItem()
    {
        if (!NetworkServer.active || dropItemPrefab == null)
        {
            return;
        }
        foreach (var dropItemSet in dropItemSets)
        {
            // 아이템 여러개 생성
            for (int i = 0; i < dropItemSet.itemCount; i++)
            {
                // 아이템 생성
                GameObject dropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
                // 아이템 값 설정
                NetworkItem item = dropItem.GetComponent<NetworkItem>();
                if (item != null)
                {
                    item.SetItemInfo(dropItemSet.itemID); // 아이템 코드 설정
                }
                // 아이템을 클라이언트모두에게 생성
                NetworkServer.Spawn(dropItem);
                // 아이템 드롭 효과 적용
                ItemDropMovement dropMovement = dropItem.GetComponent<ItemDropMovement>();
                if (dropMovement != null)
                {
                    dropMovement.InitializePosition(transform.position);
                }
            }
        }
    }
    /// <summary>
    /// 플레이어가 자원을 때렸을때, Axe 태그 비교
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AXE"))
        {
            CmdHitResource();
        }
    }

    /// <summary>
    /// 드롭할 아이템 세트 클래스
    /// </summary>
    [Serializable]
    public class DropItemSet
    {
        public string itemID; // 아이템 코드
        public int itemCount; // 드롭할 아이템 개수
    }
}