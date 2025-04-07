using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoundationUpgradeResource : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private string resourceID;
    [SerializeField] private int resourceCount;

    [Header("UI")]
    [SerializeField] private Image uiImage;
    [SerializeField] private TextMeshProUGUI uiCountText;

    private void Awake()
    {
        if (uiImage == null)
        {
            uiImage = GetComponentInChildren<Image>();
        }
        if (uiCountText == null)
        {
            uiCountText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetData(string itemID, int itemCount)
    {
        resourceID = itemID;
        resourceCount = itemCount;

        UpdateUI();
    }

    private void UpdateUI()
    {
        uiImage.sprite = Resources.Load<Sprite>("Sprites/Items/" + resourceID);
        uiCountText.text = resourceCount.ToString();
    }
}
