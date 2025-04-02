using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMItem : Item
{
    private void OnEnable()
    {
        // itemID = "6";
        // itemName = "석재";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("석재는 사용할 수 없습니다.");
    }
}
