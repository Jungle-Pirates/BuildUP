using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondItem : Item
{
    private void OnEnable()
    {
        itemID = 4;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "다이아몬드";
    }

    public override void Use()
    {
        Debug.Log("다이아몬드는 사용할 수 없습니다.");
    }
}
