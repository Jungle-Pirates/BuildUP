using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class IronSwordItem : SwordItem
{
    protected override void Awake()
    {
        base.Awake();

        // itemID = "25";
        // itemName = "철 칼";
        damage = 8f;       // 칼의 기본 피해
        delay = 0.2f;      // 피해 간격 시간
        size = 1.0f;       // 콜라이더 크기 배율
    }

    public override void Use(PlayerController user)
    {
        base.Use(user);  // 공격 처리
    }
}
