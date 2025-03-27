using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AxeItem : HandyToolItem
{
    [Header("Axe Ưȭ ����ġ")]
    [SerializeField] protected float treeDamageMultiplier = 1.5f;

    public override void Use()
    {
        GameObject target = DetectTarget();
        if (target == null)
        {
            Debug.Log("����� �����ϴ�.");
            return;
        } else if(target.tag == "Tree")
        {
            return;
        }

    }

}

