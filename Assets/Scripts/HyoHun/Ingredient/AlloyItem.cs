using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloyItem : Item
{
    private void OnEnable()
    {
        itemID = "11";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "합금";
    }

    public override void Use()
    {
        Debug.Log("합금은 사용할 수 없습니다.");
    }
}
