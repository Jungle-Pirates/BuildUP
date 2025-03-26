using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeItem : HandyToolItem
{
    [Header("Axe 특화 가중치")]
    [SerializeField] protected float treeDamageMultiplier = 1.5f;

    public override void Use()
    {
        GameObject target = DetectTarget();
        if (target == null)
        {
            Debug.Log("대상이 없습니다.");
            return;
        } else if(target.tag == "Tree")
        {
            return;
        }

    }

}

