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

        canStack = true;
        maxStackAmount = 64;
    }

    public override void Use(PlayerController user)
    {
        // TODO: 실제 회복 처리 (회복량은 최하위객체에서 받도록)

        // 인벤토리 접근하여 현재 장착된 슬롯 정보 확인 후 제거
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("InventoryManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // 0번 슬롯이 자신인지 확인 후 처리 (직접 선택되지 않았을 수도 있으므로)
        Slot[] slots = inventory.slots;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].inventoryItem == this)
            {
                slots[i].inventoryItem.count--;
                if (slots[i].inventoryItem.count <= 0)
                {
                    slots[i].inventoryItem = null;
                }

                inventory.UpdateUI();
                return;
            }
        }
    }
}
