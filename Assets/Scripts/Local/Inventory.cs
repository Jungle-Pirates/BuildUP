using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("인벤토리 내부 아이템 목록")]
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>();

    [Header("인벤토리 UI")]
    [SerializeField]
    private Transform itemUIPrefab; // 아이템 UI 프리팹
    private GameObject inventoryUI; // 인벤토리 UI
    private Transform itemUIGrid; // 아이템 UI 그리드

    void Start()
    {
        // 내 캐릭터가 아니라면 인벤토리 비활성화
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
    /// 인벤토리에 아이템 추가
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="name">아이템 이름</param>
    /// <param name="count">아이템 개수</param>
    public void AddItem(string id, string name, int count)
    {
        // 여기에 실제 인벤토리 로직 구현
        if (items.Exists(i => i.id == id))
        {
            Debug.Log($"인벤토리에 아이템 추가: {id} x{count}");
            items.Find(i => i.id == id).AddCount(count);
        }
        else
        {
            items.Add(new InventoryItem { id = id, name = name, count = count });
        }

        // 알림 메시지 표시
        Notification.Instance.CreateNotification($"<b><color=green>{name}</color></b> 획득 X {count}");
        // 인벤토리 UI 갱신
        UpdateInventoryUI();
    }

    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <param name="count">아이템 개수</param>
    public void RemoveItem(string id, int count)
    {
        // 여기에 실제 인벤토리 로직 구현
        Debug.Log($"인벤토리에서 아이템 제거: {id} x{count}");

        // 제거할 아이템 찾기
        InventoryItem item = items.Find(i => i.id == id);
        if (item.count > count)
        {
            item.count -= count;
        }
        else
        {
            items.Remove(item);
        }
        // 인벤토리 UI 갱신
        UpdateInventoryUI();
    }

    /// <summary>
    /// 인벤토리 UI 갱신
    /// </summary>
    private void UpdateInventoryUI()
    {
        // 여기에 실제 인벤토리 UI 갱신 로직 구현
        foreach (Transform child in itemUIGrid.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem item in items)
        {
            Transform itemUI = Instantiate(itemUIPrefab, itemUIGrid);
            //이미지
            Image image = itemUI.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>($"Items/{item.id}");

            //갯수
            TextMeshProUGUI itemCount = itemUI.GetComponentInChildren<TextMeshProUGUI>();
            itemCount.text = item.count.ToString();
        }
    }
}
/// <summary>
/// 인벤토리 아이템 저장 구조
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
