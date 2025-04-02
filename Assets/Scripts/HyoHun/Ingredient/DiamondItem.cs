using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondItem : Item
{
    private void OnEnable()
    {
        // itemID = "4";
        // itemName = "다이아몬드";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("다이아몬드는 사용할 수 없습니다.");
    }
}