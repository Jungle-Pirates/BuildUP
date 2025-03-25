using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Ingredient,
    Equipment,
    Food,
    ETC
}

public enum Equipable
{
    None,
    Hand,
    Leg,
    Body,
    Head
}

public abstract class Item : MonoBehaviour
{
    public int itemID;
    public ItemType itemType;
    public Equipable equipable;
    public string itemName;
    public Sprite icon;

    public abstract void Use();
}