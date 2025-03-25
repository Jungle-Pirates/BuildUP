using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeatherItem : Item
{
    private void OnEnable()
    {
        itemID = 5;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "가죽";
    }

    public override void Use()
    {
        Debug.Log("가죽은 사용할 수 없습니다.");
    }
}