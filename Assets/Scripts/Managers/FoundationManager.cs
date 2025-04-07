using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundationManager : NetworkBehaviour
{
    public static FoundationManager Instance { get; private set; }

    [SyncVar]
    [Tooltip("현재 토대 레벨")]
    [SerializeField] private int currentFoundationLevel;
    public int CurrentFoundationLevel { get { return currentFoundationLevel; } }
    [Tooltip("토대 레벨 당 도달 가능 최대 높이")]
    [SerializeField] private int[] foundationMaxHeights;
    public int[] FoundationMaxHeights { get { return foundationMaxHeights; } }
    [Tooltip("토대 업그레이드 소모 아이템")]
    [SerializeField] private RequiredItemArray[] upgradeRequiredItems;
    public RequiredItemArray[] UpgradeRequiredItems { get { return upgradeRequiredItems; } }
    public int MaxFoundationLevel => foundationMaxHeights.Length;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 현재 토대 레벨에서 업그레이드에 필요한 아이템 배열 반환
    /// </summary>
    /// <returns></returns>
    public RequiredItem[] GetRequiredItem()
    {
        return upgradeRequiredItems[currentFoundationLevel].items;
    }

    /// <summary>
    /// 토대 업그레이드 메서드
    /// - 업그레이드 성공 시 true 반환
    /// - 업그레이드 조건 불 충족 시 false 반환
    /// </summary>
    /// <returns>true: 업그레이드 성공, false: 업그레이드 실패</returns>
    public bool FoundationLevelUp()
    {
        if (HasResourceToBuild())
        {
            UseRequiredItem();
            currentFoundationLevel++;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 현재 레벨에서 업그레이드에 필요한 아이템을 소모하는 메서드
    /// </summary>
    private void UseRequiredItem()
    {
        foreach (RequiredItem requiredItem in upgradeRequiredItems[currentFoundationLevel].items)
        {
            InventoryManager.Instance.RemoveItem(requiredItem.itemID, requiredItem.amount);
        }
    }

    /// <summary>
    /// 현재 레벌에서 업그레이드에 필요한 아이템을 가지고 있는지 여부를 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    private bool HasResourceToBuild()
    {
        foreach (RequiredItem requiredItem in upgradeRequiredItems[currentFoundationLevel].items)
        {
            if (!InventoryManager.Instance.HasItemAmount(requiredItem.itemID, requiredItem.amount))
                return false;
        }
        return true;
    }
}

[System.Serializable]
public class RequiredItemArray
{
    public RequiredItem[] items;
}
