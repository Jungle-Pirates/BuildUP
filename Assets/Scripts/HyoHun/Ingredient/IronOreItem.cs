using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOreItem : Item
{
    private void OnEnable()
    {
        // itemID = "2";
        // itemName = "철광석";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("철광석은 사용할 수 없습니다.");
    }
}
