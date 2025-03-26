using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("�κ��丮 ���� ������ ���")]
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>();

    [Header("�κ��丮 UI")]
    [SerializeField]
    private Transform itemUIPrefab; // ������ UI ������
    private GameObject inventoryUI; // �κ��丮 UI
    private Transform itemUIGrid; // ������ UI �׸���

    void Start()
    {
        // �� ĳ���Ͱ� �ƴ϶�� �κ��丮 ��Ȱ��ȭ
        if (!GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            enabled = false;
            return;
        }
        inventoryUI = GameObject.Find("InventoryUI");
        itemUIGrid = inventoryUI.transform.Find("ItemGrid");
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        if(!GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
    /// <summary>
    /// �κ��丮�� ������ �߰�
    /// </summary>
    /// <param name="id">������ ID</param>
    /// <param name="name">������ �̸�</param>
    /// <param name="count">������ ����</param>
    public void AddItem(string id, string name, int count)
    {
        // ���⿡ ���� �κ��丮 ���� ����
        if (items.Exists(i => i.id == id))
        {
            Debug.Log($"�κ��丮�� ������ �߰�: {id} x{count}");
            items.Find(i => i.id == id).AddCount(count);
        }
        else
        {
            items.Add(new InventoryItem { id = id, name = name, count = count });
        }

        // �˸� �޽��� ǥ��
        Notification.Instance.CreateNotification($"<b><color=green>{name}</color></b> ȹ�� X {count}");
        // �κ��丮 UI ����
        UpdateInventoryUI();
    }

    /// <summary>
    /// �κ��丮���� ������ ����
    /// </summary>
    /// <param name="id">������ ID</param>
    /// <param name="count">������ ����</param>
    public void RemoveItem(string id, int count)
    {
        // ���⿡ ���� �κ��丮 ���� ����
        Debug.Log($"�κ��丮���� ������ ����: {id} x{count}");

        // ������ ������ ã��
        InventoryItem item = items.Find(i => i.id == id);
        if (item.count > count)
        {
            item.count -= count;
        }
        else
        {
            items.Remove(item);
        }
        // �κ��丮 UI ����
        UpdateInventoryUI();
    }

    /// <summary>
    /// �κ��丮 UI ����
    /// </summary>
    private void UpdateInventoryUI()
    {
        // ���⿡ ���� �κ��丮 UI ���� ���� ����
        foreach (Transform child in itemUIGrid.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem item in items)
        {
            Transform itemUI = Instantiate(itemUIPrefab, itemUIGrid);
            //�̹���
            Image image = itemUI.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>($"Items/{item.id}");

            //����
            TextMeshProUGUI itemCount = itemUI.GetComponentInChildren<TextMeshProUGUI>();
            itemCount.text = item.count.ToString();
        }
    }
}
/// <summary>
/// �κ��丮 ������ ���� ����
/// </summary>
[Serializable]
public class InventoryItem
{
    public string id;
    public string name;
    public int count;
    public void AddCount(int value)
    {
        count += value;
    }
}
