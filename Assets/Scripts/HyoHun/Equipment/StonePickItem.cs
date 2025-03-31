using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StonePickItem : PickItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = "21";
        itemName = "µπ ∞Ó±™¿Ã";
        damage = 5f;
        delay = 0.2f;  
        size = 1f;    
    }

    public override void Use(PlayerController user)
    {
        base.Use(user); // AxeItem¿« Use »£√‚
    }
}
