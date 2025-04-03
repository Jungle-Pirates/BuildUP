using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodMItem : Item
{
    private void Awake()
    {
        // itemID = "7";
        // itemName = "목재";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("목재는 사용할 수 없습니다.");
    }
}
