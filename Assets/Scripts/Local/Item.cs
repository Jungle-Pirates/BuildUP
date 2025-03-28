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
    public string itemID;
    public ItemType itemType;
    public Equipable equipable;
    public string itemName;
    public Sprite icon;

    public bool canStack;
    public int maxStackAmount;
    public int count;
    //생성자에서 초기화 해주는 함수 생성
    public abstract void Use();
}