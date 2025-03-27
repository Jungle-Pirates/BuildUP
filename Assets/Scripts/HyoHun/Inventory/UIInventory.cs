using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

public class UIInventory : MonoBehaviour
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

    private PlayerController_Hh controller; // �÷��̾� ��Ʈ�ѷ� ����

    void Start()
    {
        // �÷��̾� ���� �� �̺�Ʈ ���
        controller = GameObject.Find("Player").GetComponent<PlayerController_Hh>();
        dropPosition = controller.transform;
        controller.inventory += Toggle;
        controller.addItem += AddItem;

        // �ʱ�ȭ
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

    // �κ��丮 ���� �ݱ�
    public void Toggle()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeInHierarchy);
    }

    // ������ �߰� (��ø �Ǵ� �� ���Կ� ��ġ)
    public void AddItem()
    {
        Item item = controller.itemData;
        if (item == null) return;

        // Item >> InventoryItem ������ ��ȯ
        InventoryItem newItem = new InventoryItem
        {
            itemID = item.itemID,
            itemName = item.itemName,
            itemType = item.itemType,
            icon = item.icon,
            maxStackAmount = item.maxStackAmount,
            count = 1
        };

        // ��ø ������ ���� ���� Ž��
        Slot stackSlot = GetItemStack(newItem);
        if (stackSlot != null)
        {
            stackSlot.inventoryItem.AddCount(1);
            UpdateUI();
            controller.itemData = null;
            return;
        }

        // ����ִ� ���Կ� ��ġ
        Slot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.inventoryItem = newItem;
            UpdateUI();
            controller.itemData = null;
            return;
        }

        // ���� ���� >> �ٴڿ� ���
        ThrowItem(item);
        controller.itemData = null;
    }

    // ������ ������ (�̱���)
    public void ThrowItem(Item item)
    {
        // TODO: ������ ��� ������ Instantiate ó��
    }

    // UI ��ü ����
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

    // ���� ������ �������� �ִ� ���� ��ȯ (��ø ����)
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

    // ����ִ� ���� ã��
    Slot GetEmptySlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.inventoryItem == null)
                return slot;
        }
        return null;
    }

    // ������ Ŭ�� �� ������ ǥ��
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

    // �� ���� �ʱ�ȭ
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

    // ��� ��ư Ŭ�� �� ȣ��
    public void OnUseButton()
    {
        if (selectedItem.inventoryItem.itemType == ItemType.Food)
        {
            // TODO: Heal �Ǵ� ȸ�� ȿ�� ���� �ʿ�
            RemoveSelectedItem();
        }
    }

    // ������ ��ư Ŭ�� �� ȣ��
    public void OnDropButton()
    {
        ThrowItem(null); // TODO: ���� ��� ���� �� selectedItem ���
        RemoveSelectedItem();
    }

    // ������ ���� 1�� ���� �� ���� ó��
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

    // Ư�� ������ ���� ���� Ȯ��
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

// �κ��丮 �� ������ ������ Ŭ����
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
