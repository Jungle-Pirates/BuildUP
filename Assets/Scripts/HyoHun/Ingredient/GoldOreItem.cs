using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldOreItem : Item
{
    private void OnEnable()
    {
        itemID = 3;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "�ݱ���";
    }

    public override void Use()
    {
        Debug.Log("�ݱ����� ����� �� �����ϴ�.");
    }
}
