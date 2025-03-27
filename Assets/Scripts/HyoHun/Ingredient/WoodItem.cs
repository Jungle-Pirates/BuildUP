using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodItem : Item
{
    private void OnEnable()
    {
        itemID = 0;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "����";
    }

    public override void Use()
    {
        Debug.Log("������ ����� �� �����ϴ�.");
    }
}
