using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOreItem : Item
{
    private void OnEnable()
    {
        itemID = 2;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "ö����";
    }

    public override void Use()
    {
        Debug.Log("ö������ ����� �� �����ϴ�.");
    }
}
