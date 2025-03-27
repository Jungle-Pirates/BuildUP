using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiberItem : Item
{
    private void OnEnable()
    {
        itemID = 8;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "����";
    }

    public override void Use()
    {
        Debug.Log("������ ����� �� �����ϴ�.");
    }
}