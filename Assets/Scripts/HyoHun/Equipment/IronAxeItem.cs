using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = 23;
        itemName = "철 도끼";
        damage = 7f;         // 도끼의 기본 피해
        time = 1.0f;         // 피해 간격 시간
        range = 1.5f;        // 레이 사거리
        icon = null;         // 아이콘은 나중에 에디터에서 연결
    }

    public override void Use()
    {
        base.Use(); // AxeItem의 Use 호출
    }
}
