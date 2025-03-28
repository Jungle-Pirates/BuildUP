using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondItem : Item
{
    private void OnEnable()
    {
        itemID = "4";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "���̾Ƹ��";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("���̾Ƹ��� ����� �� �����ϴ�.");
    }
}