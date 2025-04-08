using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoundationUpgradeUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject maxUpgradeObject;
    [SerializeField] private List<FoundationUpgradeResource> upgradeResources = new List<FoundationUpgradeResource>();

    [Header("Resource Item")]
    [SerializeField] private Transform resourceItemParent;
    [SerializeField] private FoundationUpgradeResource resourceItemProfab;

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        upgradeResources.Clear();

        // 업그레이드 재료 피벗의 자식을 모두 삭제
        foreach (Transform child in resourceItemParent)
        {
            Destroy(child.gameObject);
        }

        if (FoundationManager.Instance.CurrentFoundationLevel >= FoundationManager.Instance.MaxFoundationLevel - 1)
        {
            maxUpgradeObject.SetActive(true);
            upgradeButton.SetActive(false);
        }
        else
        {
            maxUpgradeObject.SetActive(false);
            upgradeButton.SetActive(true);

            // 업그레이드 재료 피벗에 재료 UI 적용
            RequiredItem[] currentRequiredItems = FoundationManager.Instance.GetRequiredItem();
            for (int i = 0; i < currentRequiredItems.Length; i++)
            {
                FoundationUpgradeResource resource = Instantiate(resourceItemProfab, resourceItemParent).GetComponent<FoundationUpgradeResource>();
                resource.SetData(currentRequiredItems[i].itemID, currentRequiredItems[i].amount);
                upgradeResources.Add(resource);
            }
        }

        levelText.text = FoundationManager.Instance.CurrentFoundationLevel.ToString();
    }

    /// <summary>
    /// 토대 업그레이드 버튼을 눌렀을 때 호출하는 메서드
    /// </summary>
    public void OnPressUpgradeButton()
    {
        if (FoundationManager.Instance.FoundationLevelUp())
        {
            UpdateUI();
        }
    }
}
