using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 몬스터 종류
/// </summary>
public enum MonsterType 
{
    Boar, Chicken, Stone
}
/// <summary>
/// 몬스터 컨트롤러
/// </summary>
public class CreatureController : NetworkBehaviour
{
    [Header("몬스터 종류")]
    [SerializeField]
    private MonsterType monsterType; // 몬스터 종류

    [Space]

    [Header("몬스터 체력")]
    [SerializeField]
    private int fullHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField]
    private int currentHealth; // 현재 체력
    [SerializeField]
    private Image healthBar; // UI에서 체력을 표시할 이미지

    [Space]

    [Header("드롭 아이템 정보")]
    [SerializeField]
    private List<DropItemSet> dropItemSets = new List<DropItemSet>(); // 드롭 아이템 세트

    void Start()
    {
        currentHealth = fullHealth; // 시작 시 체력을 최대 체력으로 설정
    }
    /// <summary>
    /// 체력 변경 시 호출되는 메서드 - 체력바 업데이트
    /// </summary>
    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        currentHealth = newHealth;
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / fullHealth; // 체력 비율 계산
        }
    }
}

