using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloyItem : Item
{
    private void OnEnable()
    {
        itemID = "11";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "�ձ�";
    }

    public override void Use()
    {
        Debug.Log("�ձ��� ����� �� �����ϴ�.");
    }
}
