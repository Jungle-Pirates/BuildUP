using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitItem : FoodItem
{
    private void OnEnable()
    {
        itemID = 30;
        itemName = "����";
        healAmount = 10f;
    }

    public override void Use()
    {
        Debug.Log($"{itemName}�� �԰� ü���� {healAmount}��ŭ ȸ���ߴ�.");
        //ȸ�� ó�� 
        //�� ������ �Һ� ó��
        //PlayerHealth.Instance.Heal(healAmount);
    }
}
