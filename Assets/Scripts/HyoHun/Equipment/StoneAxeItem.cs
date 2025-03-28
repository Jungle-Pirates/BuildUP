using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAxeItem : AxeItem
{
    protected override void Awake()
    {
        base.Awake();

        itemID = 20;
        itemName = "�� ����";
        damage = 5f;         // ������ �⺻ ����
        delay = 1.2f;         // ���� ���� �ð�
        size = 1.0f;        // �ݶ��̴� ũ�� ����
        icon = null;         // �������� ���߿� �����Ϳ��� ����
    }

    public override void Use(PlayerController_Hh user)
    {
        base.Use(user); // ���� ó��
    }
}
