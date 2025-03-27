using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : Item
{
    private void OnEnable()
    {
        itemID = 10;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "�� �ֱ�";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("�� �ֱ��� ����� �� �����ϴ�.");
    }
}
