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
        itemName = "����";
    }

    public override void Use()
    {
        Debug.Log("����� ����� �� �����ϴ�.");
    }
}
