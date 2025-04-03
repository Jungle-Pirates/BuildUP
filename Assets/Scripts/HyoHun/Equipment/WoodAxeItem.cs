using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        // itemID = "Equipment_07";
        // itemName = "나무 도끼";
        damage = 3f;         
        delay = 0.2f;         // 피해 간격 시간
        size = 1.0f;        // 콜라이더 크기 배율
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // 공격 처리
    }
}