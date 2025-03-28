using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// 인벤토리 관리 클래스
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("슬롯 구성")]
    public Slot[] slots; // 슬롯 배열
    public GameObject inventoryWindow; // 인벤토리 창
    public Transform slotPanel; // 슬롯 부모
    public Transform dropPosition; // 아이템 드롭 위치

    [Header("선택된 아이템 정보 UI")]
    private Slot selectedItem; // 현재 선택한 슬롯
    private int selectedItemIndex; // 선택된 슬롯의 인덱스
    public TextMeshProUGUI selectedItemName; // 이름 표시
    public TextMeshProUGUI selectedItemType; // 타입 표시
    public TextMeshProUGUI selectedItemStatName; // 스탯명 (미사용)
    public TextMeshProUGUI selectedItemStatValue; // 스탯값 (미사용)
    public GameObject useButton; // 사용 버튼
    public GameObject dropButton; // 버리기 버튼

    void Start()
    {
        // 플레이어 연결 및 이벤트 등록
        // controller = GameObject.Find("Player").GetComponent<PlayerController_Hh>();
        // dropPosition = controller.transform;
        // controller.inventory += Toggle;
        // controller.addItem += AddItem;

        // 초기화
        slots = new Slot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<Slot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
        inventoryWindow.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Toggle(); // 인벤토리 열기/닫기
        }
    }

    /// <summary>
    /// 인벤토리 열고 닫기
    /// </summary>
    public void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }

    // 아이템 추가 (중첩 또는 새 슬롯에 배치)
    public void AddItem(string id, int count)
    {
        Item newItem = ItemPool.Instance.GetItemInstance(id);
        if (newItem == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color> 아이템 인스턴스 생성 실패: " + id);
            return; // 아이템 인스턴스가 없으면 종료
        }

        // 중첩 가능한 슬롯 먼저 탐색
        Slot stackSlot = GetItemStack(id);
        if (stackSlot != null)
        {
            stackSlot.inventoryItem.count += count; // 수량 증가
            UpdateUI();
            return;
        }

        // 비어있는 슬롯에 배치
        Slot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.inventoryItem = newItem;
            emptySlot.inventoryItem.count = count; // 수량 설정
            UpdateUI();
            return;
        }

        // 슬롯 없음 >> 바닥에 드롭
        ThrowItem(newItem);
    }

    /// <summary>
    /// 아이템 버리기
    /// </summary>
    public void ThrowItem(Item item)
    {
        // TODO: 아이템 드롭 프리팹 Instantiate 처리
    }

    /// <summary>
    /// UI 전체 갱신
    /// </summary>
    public void UpdateUI()
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null)
            {
                slot.Set();
            }
            else
            {
                slot.Clear();
            }
        }
    }

    /// <summary>
    /// 같은 종류의 아이템이 있는 슬롯 반환 (중첩 목적)
    /// </summary>
    Slot GetItemStack(string id)
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null &&
                slot.inventoryItem.itemID == id &&
                slot.inventoryItem.canStack)
            {
                return slot;
            }
        }
        return null;
    }

    /// <summary>
    /// 비어있는 슬롯 찾기
    /// </summary>
    Slot GetEmptySlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem == null)
                return slot;
        }
        return null;
    }

    /// <summary>
    /// 아이템 클릭 시 상세정보 표시
    /// </summary>
    public void SelectItem(int index)
    {
        if (slots[index].inventoryItem == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.inventoryItem.itemName;
        selectedItemType.text = selectedItem.inventoryItem.itemType.ToString();
        selectedItemStatName.text = string.Empty; // 향후 확장
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(selectedItem.inventoryItem.itemType == ItemType.Food);
        dropButton.SetActive(true);
    }

    /// <summary>
    /// 상세 정보 초기화
    /// </summary>
    void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemType.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        dropButton.SetActive(false);
    }

    /// <summary>
    /// 사용 버튼 클릭 시 호출
    /// </summary>
    public void OnUseButton()
    {
        if (selectedItem.inventoryItem.itemType == ItemType.Food)
        {
            // TODO: Heal 또는 회복 효과 연결 필요
            RemoveSelectedItem();
        }
    }

    /// <summary>
    /// 버리기 버튼 클릭 시 호출
    /// </summary>
    public void OnDropButton()
    {
        ThrowItem(null); // TODO: 실제 드롭 구현 시 selectedItem 사용
        RemoveSelectedItem();
    }

    /// <summary>
    /// 아이템 수량 1개 감소 및 제거 처리
    /// </summary>
    void RemoveSelectedItem()
    {
        selectedItem.inventoryItem.count--;
        if (selectedItem.inventoryItem.count <= 0)
        {
            selectedItem.inventoryItem = null;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }

    /// <summary>
    /// 특정 아이템 존재 여부 확인
    /// </summary>
    // public bool HasItem(int itemID, int quantity)
    // {
    //     foreach (Slot slot in slots)
    //     {
    //         if (slot.inventoryItem != null &&
    //             slot.inventoryItem.itemID == itemID &&
    //             slot.inventoryItem.count >= quantity)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
}