using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방 타입: 토대
/// </summary>
public class FoundationRoom : Room
{
    [Header("UI")]
    [SerializeField] private FoundationUpgradeUI upgradeUI;

    protected override void Start()
    {
        base.Start();
    }

    public override void OpenRoomUI()
    {
        if (IsOccupied && !isOccupiedByMe) // 방이 사용중이면 UI를 열지 않음
        {
            return;
        }
        // 농장 UI 열기
        if (upgradeUI != null)
        {
            upgradeUI.gameObject.SetActive(true);
        }
    }

    public override void CloseRoomUI()
    {
        // 농장 UI 닫기
        if (upgradeUI != null)
        {
            upgradeUI.gameObject.SetActive(false);
        }
    }
}
