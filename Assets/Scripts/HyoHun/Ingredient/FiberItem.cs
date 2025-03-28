using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiberItem : Item
{
    private void OnEnable()
    {
        itemID = "8";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "섬유";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("섬유는 사용할 수 없습니다.");
    }
}