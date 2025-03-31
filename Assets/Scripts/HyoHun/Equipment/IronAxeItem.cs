using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = "23";
        itemName = "√∂ µµ≥¢";
        damage = 8f; 
        delay = 0.2f; 
        size = 1f;
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // AxeItem¿« Use »£√‚
    }
}
