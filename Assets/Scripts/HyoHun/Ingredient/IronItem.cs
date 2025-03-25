using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronItem : Item
{
    private void OnEnable()
    {
        itemID = 9;
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "Ã¶ ÁÖ±«";
    }

    public override void Use()
    {
        Debug.Log("Ã¶ ÁÖ±«´Â »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
    }
}