using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordItem : HandyToolItem
{
    [Header("Sword 특화 가중치")]
    [SerializeField] protected float animalDamageMultiplier = 1.5f;

    public override void Use(PlayerController user)
    {
        if (attackPoint != null)
            attackPoint.tag = "SWORD"; // 도구 타입 식별용 태그

        base.Use(user);
    }
}
