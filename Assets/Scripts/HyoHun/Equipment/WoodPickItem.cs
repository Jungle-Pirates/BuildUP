using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WoodPickItem : PickItem
{
    protected override void Awake()
{
    base.Awake();

     // itemID = "Equipment_08";
     // itemName = "Equipment_08";
     damage = 3f;
    delay = 0.2f;
    size = 1f;
}

public override void Use(PlayerController user)
{
    base.Use(user);
}
}