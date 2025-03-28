using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : Singleton<ItemPool>
{
    /// <summary>
    /// 아이템 코드 + 프리팹 사전 (프리팹을 Inspector에서 설정)
    /// </summary>
    [Serializable]
    public class ItemPrefabDictionary
    {
        public string itemCode;
        public GameObject itemPrefab;
    }
    public List<ItemPrefabDictionary> itemPrefabs = new List<ItemPrefabDictionary>();
    private Dictionary<string, GameObject> itemPrefabDict = new Dictionary<string, GameObject>();

    // 아이템 인스턴스 캐싱 (각 아이템 타입당 하나의 인스턴스만 유지)
    private Dictionary<string, Item> itemInstances = new Dictionary<string, Item>();

    // 풀 저장용 Transform
    private Transform poolContainer;

    protected override void Awake()
    {
        base.Awake();

        // 풀 컨테이너 생성
        poolContainer = new GameObject("ItemPool").transform;
        poolContainer.SetParent(transform);

        // 프리팹 사전 초기화
        foreach (var item in itemPrefabs)
        {
            if (item.itemPrefab != null)
            {
                itemPrefabDict[item.itemCode] = item.itemPrefab;
            }
        }
    }

    // 아이템 인스턴스 얻기 (캐시에 있으면 가져오고, 없으면 생성)
    public Item GetItemInstance(string itemCode)
    {
        // 이미 생성된 인스턴스가 있으면 사용
        if (itemInstances.ContainsKey(itemCode))
        {
            return itemInstances[itemCode];
        }

        // 없으면 새로 생성하고 캐시에 저장
        Item newItem = CreateNewItem(itemCode);
        if (newItem != null)
        {
            itemInstances[itemCode] = newItem;
            newItem.gameObject.SetActive(false); // 비활성화 상태로 보관
            newItem.transform.SetParent(poolContainer);
        }

        return newItem;
    }

    // 새 아이템 생성
    private Item CreateNewItem(string itemCode)
    {
        // 코드에 해당하는 프리팹이 없으면 null 반환
        if (!itemPrefabDict.ContainsKey(itemCode))
        {
            Debug.LogError($"<color=red>[에러 발생]</color> 아이템 코드 {itemCode}에 해당하는 프리팹이 없습니다.");
            return null;
        }

        // 새 아이템 인스턴스 생성
        GameObject newItemObj = Instantiate(itemPrefabDict[itemCode], poolContainer);
        Item newItem = newItemObj.GetComponent<Item>();

        if (newItem == null)
        {
            Debug.LogError($"<color=red>[에러 발생]</color> 생성된 프리팹 {itemCode}에 Item 컴포넌트가 없습니다.");
            Destroy(newItemObj);
            return null;
        }

        return newItem;
    }
}