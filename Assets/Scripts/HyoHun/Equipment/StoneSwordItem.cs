using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StoneSwordItem : SwordItem
{
    protected override void Awake()
    {
        base.Awake();

        // itemID = "22";
        // itemName = "돌 칼";
        damage = 5f;         // 도끼의 기본 피해
        delay = 0.2f;         // 피해 간격 시간
        size = 1.0f;        // 콜라이더 크기 배율
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // 공격 처리
    }
}
