using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : FoodItem
{
    private void OnEnable()
    {
        itemID = "31";
        itemName = "���";
        healAmount = 25f;
    }

    public override void Use()
    {
        Debug.Log($"{itemName}�� �԰� ü���� {healAmount}��ŭ ȸ���ߴ�.");
        //ȸ�� ó�� 
        //�� ������ �Һ� ó��
        //PlayerHealth.Instance.Heal(healAmount);
    }
}
