using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Transform canvas;
    private GameObject dragGhost;               // 드래그 시 보여줄 임시 오브젝트
    private Image dragGhostImage;
    private Slot originSlot;

    public Slot OriginSlot => originSlot;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originSlot = GetComponentInParent<Slot>();
        if (originSlot == null || originSlot.inventoryItem == null) return;

        // 드래그 고스트 생성 (시각용)
        dragGhost = new GameObject("DragGhost", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragGhost.transform.SetParent(canvas);
        dragGhost.transform.SetAsLastSibling();

        RectTransform ghostRect = dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = rect.sizeDelta;
        ghostRect.position = eventData.position;

        dragGhostImage = dragGhost.GetComponent<Image>();
        dragGhostImage.sprite = originSlot.inventoryItem.icon;
        dragGhostImage.raycastTarget = false;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            dragGhost.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}