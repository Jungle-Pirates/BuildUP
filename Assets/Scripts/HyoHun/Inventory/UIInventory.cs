using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

public class UIInventory : MonoBehaviour
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

    private PlayerController_Hh controller; // 플레이어 컨트롤러 참조

    void Start()
    {
        // 플레이어 연결 및 이벤트 등록
        controller = GameObject.Find("Player").GetComponent<PlayerController_Hh>();
        dropPosition = controller.transform;
        controller.inventory += Toggle;
        controller.addItem += AddItem;

        // 초기화
        inventoryWindow.SetActive(false);
        slots = new Slot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<Slot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    // 인벤토리 열고 닫기
    public void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }

    // 아이템 추가 (중첩 또는 새 슬롯에 배치)
    public void AddItem()
    {
        Item item = controller.itemData;
        if (item == null) return;

        // Item >> InventoryItem 데이터 변환
        InventoryItem newItem = new InventoryItem
        {
            itemID = item.itemID,
            itemName = item.itemName,
            itemType = item.itemType,
            icon = item.icon,
            maxStackAmount = item.maxStackAmount,
            count = 1
        };

        // 중첩 가능한 슬롯 먼저 탐색
        Slot stackSlot = GetItemStack(newItem);
        if (stackSlot != null)
        {
            stackSlot.inventoryItem.AddCount(1);
            UpdateUI();
            controller.itemData = null;
            return;
        }

        // 비어있는 슬롯에 배치
        Slot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.inventoryItem = newItem;
            UpdateUI();
            controller.itemData = null;
            return;
        }

        // 슬롯 없음 >> 바닥에 드롭
        ThrowItem(item);
        controller.itemData = null;
    }

    // 아이템 버리기 (미구현)
    public void ThrowItem(Item item)
    {
        // TODO: 아이템 드롭 프리팹 Instantiate 처리
    }

    // UI 전체 갱신
    public void UpdateUI()
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null)
                slot.Set();
            else
                slot.Clear();
        }
    }

    // 같은 종류의 아이템이 있는 슬롯 반환 (중첩 목적)
    Slot GetItemStack(InventoryItem item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null &&
                slot.inventoryItem.itemID == item.itemID &&
                slot.inventoryItem.CanStack)
            {
                return slot;
            }
        }
        return null;
    }

    // 비어있는 슬롯 찾기
    Slot GetEmptySlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem == null)
                return slot;
        }
        return null;
    }

    // 아이템 클릭 시 상세정보 표시
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

    // 상세 정보 초기화
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

    // 사용 버튼 클릭 시 호출
    public void OnUseButton()
    {
        if (selectedItem.inventoryItem.itemType == ItemType.Food)
        {
            // TODO: Heal 또는 회복 효과 연결 필요
            RemoveSelectedItem();
        }
    }

    // 버리기 버튼 클릭 시 호출
    public void OnDropButton()
    {
        ThrowItem(null); // TODO: 실제 드롭 구현 시 selectedItem 사용
        RemoveSelectedItem();
    }

    // 아이템 수량 1개 감소 및 제거 처리
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

    // 특정 아이템 존재 여부 확인
    public bool HasItem(int itemID, int quantity)
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null &&
                slot.inventoryItem.itemID == itemID &&
                slot.inventoryItem.count >= quantity)
            {
                return true;
            }
        }
        return false;
    }
}

// 인벤토리 내 아이템 데이터 클래스
[Serializable]
public class InventoryItem
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public int count;
    public int maxStackAmount;
    public Sprite icon;

    public void AddCount(int value) => count += value;
    public bool CanStack => count < maxStackAmount;
}
