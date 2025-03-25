using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodMItem : Item
{
    private void OnEnable()
    {
        itemID = 7;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "목재";
    }

    public override void Use()
    {
        Debug.Log("목재는 사용할 수 없습니다.");
    }
}
