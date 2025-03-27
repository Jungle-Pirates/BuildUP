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
        itemName = "����";
    }

    public override void Use()
    {
        Debug.Log("������ ����� �� �����ϴ�.");
    }
}