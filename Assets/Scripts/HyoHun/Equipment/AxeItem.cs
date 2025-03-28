using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeItem : HandyToolItem
{
    [Header("Axe Ưȭ ����ġ")]
    [SerializeField] protected float treeDamageMultiplier = 1.5f;

    public override void Use(PlayerController_Hh user)
    {
        if (attackPoint != null)
            attackPoint.tag = "Axe"; // ���� Ÿ�� �ĺ��� �±�

        base.Use(user);
    }
}
