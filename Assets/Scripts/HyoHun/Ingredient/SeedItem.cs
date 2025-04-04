using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedItem : Item
{
    private void OnEnable()
    {
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        Debug.Log("씨앗은 사용할 수 없습니다.");
    }
}
