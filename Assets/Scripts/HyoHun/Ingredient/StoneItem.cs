using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneItem : Item
{
    private void OnEnable()
    {
        itemID = "1";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "돌";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("돌은 사용할 수 없습니다.");
    }
}
