using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private RectTransform rect;
    private Slot targetSlot;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        targetSlot = GetComponent<Slot>();
    }

    public void OnPointerEnter(PointerEventData eventData) { }

    public void OnPointerExit(PointerEventData eventData) { }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI draggedUI = eventData.pointerDrag?.GetComponent<DraggableUI>();
        if (draggedUI == null || draggedUI.OriginSlot == null || targetSlot == null)
            return;

        Slot fromSlot = draggedUI.OriginSlot;
        Slot toSlot = targetSlot;

        if (fromSlot == toSlot) return;

        // 아이템 정보 스왑
        Item tempItem = toSlot.inventoryItem;
        toSlot.inventoryItem = fromSlot.inventoryItem;
        fromSlot.inventoryItem = tempItem;

        // UI 갱신
        toSlot.inventory.UpdateUI();
        fromSlot.inventory.UpdateUI();
    }
}