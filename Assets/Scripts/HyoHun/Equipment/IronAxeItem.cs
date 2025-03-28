using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = 23;
        itemName = "ö ����";
        damage = 7f;         // ������ �⺻ ����
        time = 1.0f;         // ���� ���� �ð�
        range = 1.5f;        // ���� ��Ÿ�
        icon = null;         // �������� ���߿� �����Ϳ��� ����
    }

    public override void Use()
    {
        base.Use(); // AxeItem�� Use ȣ��
    }
}
