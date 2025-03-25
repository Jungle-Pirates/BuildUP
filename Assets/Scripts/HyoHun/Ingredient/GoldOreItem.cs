using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldOreItem : Item
{
    private void OnEnable()
    {
        itemID = 3;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "금광석";
    }

    public override void Use()
    {
        Debug.Log("금광석은 사용할 수 없습니다.");
    }
}
