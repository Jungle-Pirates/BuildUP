using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneItem : Item
{
    private void OnEnable()
    {
        // itemID = "1";
        // itemName = "돌";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("돌은 사용할 수 없습니다.");
    }
}
