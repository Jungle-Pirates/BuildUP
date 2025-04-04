using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrainItem : Item
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
        Debug.Log("곡물은 사용할 수 없습니다.");
    }
}
