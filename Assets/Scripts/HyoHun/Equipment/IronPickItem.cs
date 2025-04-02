using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.Progress;

public class IronPickItem : PickItem
{
    protected override void Awake()
    {
        base.Awake();

        // itemID = "24";
        // itemName = "철 곡괭이";
        damage = 8f;         // 도끼의 기본 피해
        delay = 0.2f;         // 피해 간격 시간
        size = 1.0f;        // 콜라이더 크기 배율
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // 공격 처리
    }
}
