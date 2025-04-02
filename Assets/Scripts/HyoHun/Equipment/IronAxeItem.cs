using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        // itemID = "23";
        // itemName = "철 도끼";
        damage = 8f; 
        delay = 0.2f; 
        size = 1f;
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // AxeItem의 Use 호출
    }
}
