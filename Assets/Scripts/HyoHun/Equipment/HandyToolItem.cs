using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �տ� ���� ������ ������ ���� Ŭ����
/// </summary>
public abstract class HandyToolItem : Item
{
    [Header("Handy Tool Stats")]
    [SerializeField] protected float damage;        // �⺻ ���ط�
    [SerializeField] protected float delay;          // ������ �ð�
    [SerializeField] protected float size;         // �ݶ��̴� ũ�� ����?
    [SerializeField] protected bool isUsing;        // ��� �� ����

    protected GameObject attackPoint;               // �÷��̾��� AttackPoint �ݶ��̴� ����
    protected Collider2D attackCollider;            // ���� �ݶ��̴� ĳ��

    protected virtual void Awake()
    {
        itemType = ItemType.Equipment;
        equipable = Equipable.Hand;

        canStack = false;
        maxStackAmount = 1;

        isUsing = false;
    }

    /// <summary>
    /// ���� ���: ���� �ð� ���� AttackPoint �ݶ��̴��� �Ѱ� ���� ActivateAttackPoint �ڷ�ƾ ȣ��
    /// </summary>
    public override void Use(PlayerController_Hh user)
    {
        if (isUsing || user == null) return;

        attackPoint = user.attackPoint;
        attackCollider = attackPoint?.GetComponent<Collider2D>();

        if (attackCollider == null)
        {
            Debug.LogWarning("AttackCollider�� �����ϴ�.");
            return;
        }

        user.StartCoroutine(ActivateAttackPoint()); // Coroutine ����
    }

    /// <summary>
    /// AttackPoint �ݶ��̴��� ���� �ð� ���� Ȱ��ȭ�ϴ� �ڷ�ƾ
    /// </summary>
    protected IEnumerator ActivateAttackPoint()
    {
        isUsing = true;

        // �ݶ��̴� ����
        attackCollider.enabled = true;
        attackCollider.transform.localScale = Vector3.one * size;

        yield return new WaitForSeconds(delay);

        attackCollider.enabled = false; // �浹 ����

        // Ȥ�� ���� �±� �ʱ�ȭ
        attackPoint.tag = "Untagged";

        isUsing = false;
    }
}