using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItem : HandyToolItem
{
    [Header("Pick 특화 가중치")]
    [SerializeField] protected float rockDamageMultiplier = 1.5f;

    public override void Use(PlayerController user)
    {
        if (attackPoint != null)
            attackPoint.tag = "PICK"; // 도구 타입 식별용 태그

        base.Use(user);
    }
}
