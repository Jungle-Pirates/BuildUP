using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// �κ��丮 ���� Ŭ����
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("���� ����")]
    public Slot[] slots; // ���� �迭
    public GameObject inventoryWindow; // �κ��丮 â
    public Transform slotPanel; // ���� �θ�
    public Transform dropPosition; // ������ ��� ��ġ

    [Header("���õ� ������ ���� UI")]
    private Slot selectedItem; // ���� ������ ����
    private int selectedItemIndex; // ���õ� ������ �ε���
    public TextMeshProUGUI selectedItemName; // �̸� ǥ��
    public TextMeshProUGUI selectedItemType; // Ÿ�� ǥ��
    public TextMeshProUGUI selectedItemStatName; // ���ȸ� (�̻��)
    public TextMeshProUGUI selectedItemStatValue; // ���Ȱ� (�̻��)
    public GameObject useButton; // ��� ��ư
    public GameObject dropButton; // ������ ��ư

    void Start()
    {
        // �÷��̾� ���� �� �̺�Ʈ ���
        // controller = GameObject.Find("Player").GetComponent<PlayerController_Hh>();
        // dropPosition = controller.transform;
        // controller.inventory += Toggle;
        // controller.addItem += AddItem;

        // �ʱ�ȭ
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
            Toggle(); // �κ��丮 ����/�ݱ�
        }
    }

    /// <summary>
    /// �κ��丮 ���� �ݱ�
    /// </summary>
    public void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }

    // ������ �߰� (��ø �Ǵ� �� ���Կ� ��ġ)
    public void AddItem(string id, int count)
    {
        Item newItem = ItemPool.Instance.GetItemInstance(id);
        if (newItem == null)
        {
            Debug.LogError("<color=red>[���� �߻�]</color> ������ �ν��Ͻ� ���� ����: " + id);
            return; // ������ �ν��Ͻ��� ������ ����
        }

        // ��ø ������ ���� ���� Ž��
        Slot stackSlot = GetItemStack(id);
        if (stackSlot != null)
        {
            stackSlot.inventoryItem.count += count; // ���� ����
            UpdateUI();
            return;
        }

        // ����ִ� ���Կ� ��ġ
        Slot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.inventoryItem = newItem;
            emptySlot.inventoryItem.count = count; // ���� ����
            UpdateUI();
            return;
        }

        // ���� ���� >> �ٴڿ� ���
        ThrowItem(newItem);
    }

    /// <summary>
    /// ������ ������
    /// </summary>
    public void ThrowItem(Item item)
    {
        // TODO: ������ ��� ������ Instantiate ó��
    }

    /// <summary>
    /// UI ��ü ����
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
    /// ���� ������ �������� �ִ� ���� ��ȯ (��ø ����)
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
    /// ����ִ� ���� ã��
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
    /// ������ Ŭ�� �� ������ ǥ��
    /// </summary>
    public void SelectItem(int index)
    {
        if (slots[index].inventoryItem == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.inventoryItem.itemName;
        selectedItemType.text = selectedItem.inventoryItem.itemType.ToString();
        selectedItemStatName.text = string.Empty; // ���� Ȯ��
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(selectedItem.inventoryItem.itemType == ItemType.Food);
        dropButton.SetActive(true);
    }

    /// <summary>
    /// �� ���� �ʱ�ȭ
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
    /// ��� ��ư Ŭ�� �� ȣ��
    /// </summary>
    public void OnUseButton()
    {
        if (selectedItem.inventoryItem.itemType == ItemType.Food)
        {
            // TODO: Heal �Ǵ� ȸ�� ȿ�� ���� �ʿ�
            RemoveSelectedItem();
        }
    }

    /// <summary>
    /// ������ ��ư Ŭ�� �� ȣ��
    /// </summary>
    public void OnDropButton()
    {
        ThrowItem(null); // TODO: ���� ��� ���� �� selectedItem ���
        RemoveSelectedItem();
    }

    /// <summary>
    /// ������ ���� 1�� ���� �� ���� ó��
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
    /// Ư�� ������ ���� ���� Ȯ��
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