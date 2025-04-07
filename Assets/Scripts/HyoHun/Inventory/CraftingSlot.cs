using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    private CraftingRoom craftingRoom;
    public string itemID;
    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickSlot);
        craftingRoom = GetComponentInParent<CraftingRoom>();
        if (craftingRoom == null)
        {
            Debug.LogError("CraftingRoom component not found in parent.");
        }
    }

    public void OnClickSlot()
    {
        // Check if the crafting room is not null before proceeding
        if (craftingRoom != null)
        {
            craftingRoom.AddCraftingQueue(itemID);
        }
        else
        {
            Debug.LogError("CraftingRoom reference is null.");
        }
    }
}
