using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : Singleton<ItemPool>
{
    /// <summary>
    /// ������ �ڵ� + ������ ���� (�������� Inspector���� ����)
    /// </summary>
    [Serializable]
    public class ItemPrefabDictionary
    {
        public string itemCode;
        public GameObject itemPrefab;
    }
    public List<ItemPrefabDictionary> itemPrefabs = new List<ItemPrefabDictionary>();
    private Dictionary<string, GameObject> itemPrefabDict = new Dictionary<string, GameObject>();

    // ������ �ν��Ͻ� ĳ�� (�� ������ Ÿ�Դ� �ϳ��� �ν��Ͻ��� ����)
    private Dictionary<string, Item> itemInstances = new Dictionary<string, Item>();

    // Ǯ ����� Transform
    private Transform poolContainer;

    protected override void Awake()
    {
        base.Awake();

        // Ǯ �����̳� ����
        poolContainer = new GameObject("ItemPool").transform;
        poolContainer.SetParent(transform);

        // ������ ���� �ʱ�ȭ
        foreach (var item in itemPrefabs)
        {
            if (item.itemPrefab != null)
            {
                itemPrefabDict[item.itemCode] = item.itemPrefab;
            }
        }
    }

    // ������ �ν��Ͻ� ��� (ĳ�ÿ� ������ ��������, ������ ����)
    public Item GetItemInstance(string itemCode)
    {
        // �̹� ������ �ν��Ͻ��� ������ ���
        if (itemInstances.ContainsKey(itemCode))
        {
            return itemInstances[itemCode];
        }

        // ������ ���� �����ϰ� ĳ�ÿ� ����
        Item newItem = CreateNewItem(itemCode);
        if (newItem != null)
        {
            itemInstances[itemCode] = newItem;
            newItem.gameObject.SetActive(false); // ��Ȱ��ȭ ���·� ����
            newItem.transform.SetParent(poolContainer);
        }

        return newItem;
    }

    // �� ������ ����
    private Item CreateNewItem(string itemCode)
    {
        // �ڵ忡 �ش��ϴ� �������� ������ null ��ȯ
        if (!itemPrefabDict.ContainsKey(itemCode))
        {
            Debug.LogError($"<color=red>[���� �߻�]</color> ������ �ڵ� {itemCode}�� �ش��ϴ� �������� �����ϴ�.");
            return null;
        }

        // �� ������ �ν��Ͻ� ����
        GameObject newItemObj = Instantiate(itemPrefabDict[itemCode], poolContainer);
        Item newItem = newItemObj.GetComponent<Item>();

        if (newItem == null)
        {
            Debug.LogError($"<color=red>[���� �߻�]</color> ������ ������ {itemCode}�� Item ������Ʈ�� �����ϴ�.");
            Destroy(newItemObj);
            return null;
        }

        return newItem;
    }
}