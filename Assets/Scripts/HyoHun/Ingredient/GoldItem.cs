using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : Item
{
    private void OnEnable()
    {
        // itemID = "10";
        // itemName = "금 주괴";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("금 주괴는 사용할 수 없습니다.");
    }
}
