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
        itemName = "���̾Ƹ��";
    }

    public override void Use()
    {
        Debug.Log("���̾Ƹ��� ����� �� �����ϴ�.");
    }
}
