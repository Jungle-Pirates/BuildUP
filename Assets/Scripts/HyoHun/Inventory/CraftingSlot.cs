using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    public string itemID;
    public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickSlot);
    }

    public void OnClickSlot()
    {
        InventoryManager.Instance.SelectCraftingRecipe(itemID);
    }
}
