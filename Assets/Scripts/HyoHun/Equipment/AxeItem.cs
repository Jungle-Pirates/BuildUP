using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeItem : HandyToolItem
{
    [Header("Axe 특화 가중치")]
    [SerializeField] protected float treeDamageMultiplier = 1.5f;

    public override void Use(PlayerController_Hh user)
    {
        if (attackPoint != null)
            attackPoint.tag = "Axe"; // 도구 타입 식별용 태그

        base.Use(user);
    }
}
