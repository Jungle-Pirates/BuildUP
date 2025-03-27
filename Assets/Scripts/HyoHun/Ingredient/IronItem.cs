using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronItem : Item
{
    private void OnEnable()
    {
        itemID = 9;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "ö �ֱ�";
    }

    public override void Use()
    {
        Debug.Log("ö �ֱ��� ����� �� �����ϴ�.");
    }
}