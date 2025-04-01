using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronItem : Item
{
    private void OnEnable()
    {
        itemID = "9";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "철 주괴";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("철 주괴는 사용할 수 없습니다.");
    }
}