using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HandyToolItem : Item
{
    [Header("Handy Tool Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float time;
    [SerializeField] protected float radius;

    protected virtual void Awake()
    {
        itemType = ItemType.Equipment;
        equipable = Equipable.Hand;

        canStack = false;
        maxStackAmount = 1;
    }

    public override abstract void Use();

    protected GameObject DetectTarget()
    {
        //�������� ���̰� ������ �ϴµ�, �� ��� ������ ������
        Vector2 origin = transform.position;
        Vector2 direction = transform.right * Mathf.Sign(transform.localScale.x);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, radius);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
