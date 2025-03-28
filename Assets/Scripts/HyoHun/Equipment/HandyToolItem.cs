using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 손에 장착 가능한 아이템 상위 클래스
/// </summary>
public abstract class HandyToolItem : Item
{
    [Header("Handy Tool Stats")]
    [SerializeField] protected float damage;        // 기본 피해량
    [SerializeField] protected float delay;          // 딜레이 시간
    [SerializeField] protected float size;         // 콜라이더 크기 배율?
    [SerializeField] protected bool isUsing;        // 사용 중 여부

    protected GameObject attackPoint;               // 플레이어의 AttackPoint 콜라이더 참조
    protected Collider2D attackCollider;            // 실제 콜라이더 캐싱

    protected virtual void Awake()
    {
        itemType = ItemType.Equipment;
        equipable = Equipable.Hand;

        canStack = false;
        maxStackAmount = 1;

        isUsing = false;
    }

    /// <summary>
    /// 도구 사용: 일정 시간 동안 AttackPoint 콜라이더를 켜고 끄는 ActivateAttackPoint 코루틴 호출
    /// </summary>
    public override void Use(PlayerController_Hh user)
    {
        if (isUsing || user == null) return;

        attackPoint = user.attackPoint;
        attackCollider = attackPoint?.GetComponent<Collider2D>();

        if (attackCollider == null)
        {
            Debug.LogWarning("AttackCollider가 없습니다.");
            return;
        }

        user.StartCoroutine(ActivateAttackPoint()); // Coroutine 실행
    }

    /// <summary>
    /// AttackPoint 콜라이더를 일정 시간 동안 활성화하는 코루틴
    /// </summary>
    protected IEnumerator ActivateAttackPoint()
    {
        isUsing = true;

        // 콜라이더 세팅
        attackCollider.enabled = true;
        attackCollider.transform.localScale = Vector3.one * size;

        yield return new WaitForSeconds(delay);

        attackCollider.enabled = false; // 충돌 종료

        // 혹시 몰라서 태그 초기화
        attackPoint.tag = "Untagged";

        isUsing = false;
    }
}