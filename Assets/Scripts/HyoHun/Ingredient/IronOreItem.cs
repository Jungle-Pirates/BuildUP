using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronOreItem : Item
{
    private void OnEnable()
    {
        itemID = "2";
        itemType = ItemType.Ingredient;
        equipable = Equipable.None;
        itemName = "Ã¶±¤¼®";

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use()
    {
        Debug.Log("Ã¶±¤¼®Àº »ç¿ëÇÒ ¼ö ¾ø½À´Ï´Ù.");
    }
}
