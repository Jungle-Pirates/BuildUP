using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;
using Mirror;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 관리 클래스
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("슬롯 구성")]
    public Slot[] slots; // 슬롯 배열
    public GameObject inventoryWindow; // 인벤토리 창
    public Transform slotPanel; // 슬롯 부모

    [Header("선택된 아이템 정보 UI")]
    private Slot selectedItem; // 현재 선택한 슬롯
    private int selectedItemIndex; // 선택된 슬롯의 인덱스
    public TextMeshProUGUI selectedItemName; // 이름 표시
    public TextMeshProUGUI selectedItemType; // 타입 표시
    public TextMeshProUGUI selectedItemStatName; // 스탯명 (미사용)
    public TextMeshProUGUI selectedItemStatValue; // 스탯값 (미사용)
    public Button useButton; // 사용 버튼
    public Button dropButton; // 버리기 버튼

    [Header("제작 UI")]
    public GameObject craftingPanel; // 끄고 켤 수 있는 제작 UI 묶음
    public Button craftButton;
    public TextMeshProUGUI craftWarningText;
    private string selectedCraftItemID = null;

    void Start()
    {
        // 플레이어 연결 및 이벤트 등록
        // controller = GameObject.Find("Player").GetComponent<PlayerController>();
        // dropPosition = controller.transform;
        // controller.inventory += Toggle;
        // controller.addItem += AddItem;
        dropButton.onClick.AddListener(OnDropButton);

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

        craftButton.gameObject.SetActive(false);
        craftWarningText.gameObject.SetActive(false);
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
        Item newItem = ItemDataPool.Instance.GetItemInstance(id);
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
        NetworkClient.localPlayer.GetComponent<PlayerController>().ThrowItem(newItem.itemID);
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

        //제작 관련 버튼 및 텍스트는 일반 인벤토리 눌렀을 땐 꺼져야 함 
        craftButton.gameObject.SetActive(false);
        craftWarningText.gameObject.SetActive(false);

        useButton.gameObject.SetActive(selectedItem.inventoryItem.itemType == ItemType.Food);
        dropButton.gameObject.SetActive(true);
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

        useButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 사용 버튼 클릭 시 호출 (일단 Food만 뜸)
    /// </summary>
    public void OnUseButton()
    {
        if (selectedItem == null || selectedItem.inventoryItem == null)
            return;

        // 아이템 내부의 Use(PlayerController user) 호출
        selectedItem.inventoryItem.Use(NetworkClient.localPlayer.GetComponent<PlayerController>());
    }

    /// <summary>
    /// 버리기 버튼 클릭 시 호출
    /// </summary>
    public void OnDropButton()
    {
        NetworkClient.localPlayer.GetComponent<PlayerController>().ThrowItem(selectedItem.inventoryItem.itemID);
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
    /// 인벤토리에서 특정 아이템을 지정된 수량만큼 제거
    /// </summary>
    public void RemoveItem(string itemID, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null && slot.inventoryItem.itemID == itemID)
            {
                int count = slot.inventoryItem.count;

                if (count >= remaining)
                {
                    slot.inventoryItem.count -= remaining;

                    if (slot.inventoryItem.count <= 0)
                        slot.inventoryItem = null;

                    break;
                }
                else
                {
                    remaining -= count;
                    slot.inventoryItem = null;
                }
            }
        }

        UpdateUI();
    }

    /// <summary>
    /// 특정 아이템 존재 여부 확인
    /// </summary>
    public bool HasItemAmount(string itemID, int requiredAmount)
    {
        int totalAmount = 0;

        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem != null && slot.inventoryItem.itemID == itemID)
            {
                totalAmount += slot.inventoryItem.count;
                if (totalAmount >= requiredAmount)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 제작 UI 슬롯 클릭시 호출
    /// </summary>
    public void SelectCraftingRecipe(string itemID)
    {
        selectedCraftItemID = itemID;

        Recipe recipe = CraftingManager.Instance.GetRecipeByItemID(itemID);
        if (recipe == null)
        {
            Debug.LogError($"레시피 없음: {itemID}");
            craftButton.gameObject.SetActive(false);
            craftWarningText.text = "레시피를 찾을 수 없습니다.";
            return;
        }

        // UI 업데이트
        selectedItemName.text = recipe.recipeName;
        selectedItemType.text = "Craft Result"; //TODO : 이거 제작 UI 슬롯이 해당 아이템 스크립트 가지도록 해서 Item 참조해서 ItemType 받아오도록 수정하자.

        bool canCraft = CraftingManager.Instance.IsAbleToCraft(recipe);
        craftButton.gameObject.SetActive(canCraft);
        craftWarningText.text = canCraft ? "" : "재료가 부족합니다.";
        craftWarningText.gameObject.SetActive(!canCraft);
        dropButton.gameObject.SetActive(false);
    }
}