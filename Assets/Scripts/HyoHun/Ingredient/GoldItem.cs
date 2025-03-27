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
    }

    public override void Use()
    {
        Debug.Log("�� �ֱ��� ����� �� �����ϴ�.");
    }
}
