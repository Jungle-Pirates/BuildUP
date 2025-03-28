using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMItem : Item
{
    private void OnEnable()
    {
        itemID = "6";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "석재";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("석재는 사용할 수 없습니다.");
    }
}
