using System;
using UnityEngine;

[Serializable]
public class RequiredItem
{
    public string itemID;
    public int amount;
}

[CreateAssetMenu(fileName = "New CraftableItem", menuName = "Craft/CraftableItem")]
public class Recipe : ScriptableObject
{
    [Header("제작 레시피 정보")]
    public string recipeName;
    [Tooltip("완성품 아이템 코드")]
    public string resultItemID;
    [Tooltip("이 아이템 제작에 필요한 재료 목록")]
    public RequiredItem[] requiredItem;
    [Tooltip("제작 결과로 얻는 아이템 수량")]
    public int craftAmount = 1;
    [Tooltip("제작에 필요한 시간 (초)")]
    public float craftTime = 2f;
}