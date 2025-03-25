using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FoodItem : Item
{
    protected float healAmount;

    protected virtual void Awake()
    {
        itemType = ItemType.Food;
        equipable = Equipable.Hand;
    }

    public override abstract void Use();
}
