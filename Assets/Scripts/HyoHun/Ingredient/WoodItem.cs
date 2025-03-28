using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodItem : Item
{
    private void OnEnable()
    {
        itemID = "0";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "나무";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("나무는 사용할 수 없습니다.");
    }
}
