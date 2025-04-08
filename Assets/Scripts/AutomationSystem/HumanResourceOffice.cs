using UnityEngine;
using Mirror;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using TMPro;

/// <summary>
/// 실질적으로 인력값을 저정하는 클래스
/// </summary>
public class HumanResourceOffice : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI totalWorkersText; // 전체 일꾼 수 텍스트
    [SerializeField]
    private TextMeshProUGUI availableWorkersText; // 사용 가능한 일꾼 수 텍스트

    // 전체 일꾼 수
    [SyncVar(hook = nameof(OnTotalWorkersChanged))]
    public int totalWorkers = 0;

    // 현재 사용 가능한 일꾼 수
    [SyncVar(hook = nameof(OnAvailableWorkersChanged))]
    public int availableWorkers = 0;

    public void OnTotalWorkersChanged(int oldValue, int newValue)
    {
        // UI 업데이트 또는 다른 작업 수행
        totalWorkersText.text = newValue.ToString();
    }
    public void OnAvailableWorkersChanged(int oldValue, int newValue)
    {
        // UI 업데이트 또는 다른 작업 수행
        availableWorkersText.text = newValue.ToString();
    }
}