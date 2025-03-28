using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = 20;
        itemName = "돌 도끼";
        damage = 5f;         // 도끼의 기본 피해
        delay = 1.2f;         // 피해 간격 시간
        size = 1.0f;        // 콜라이더 크기 배율
        icon = null;         // 아이콘은 나중에 에디터에서 연결
    }

    public override void Use(PlayerController_Hh user)
    {
        base.Use(user); // 공격 처리
    }
}
