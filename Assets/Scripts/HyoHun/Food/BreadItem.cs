using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadItem : FoodItem
{
    private void OnEnable()
    {
        healAmount = 30f;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log($"{itemName}을 먹고 체력을 {healAmount}만큼 회복했다.");
        //회복 처리 
        //이 아이템 소비 처리
        //PlayerHealth.Instance.Heal(healAmount);
    }
}
