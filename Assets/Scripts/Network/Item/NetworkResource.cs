using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// �ڿ� Ŭ���� (����, ����...)
/// </summary>
public class NetworkResource : NetworkBehaviour
{
    [Header("�ڿ� ����")]
    [Tooltip("����� ������ ������")]
    [SerializeField]
    private GameObject dropItemPrefab;
    [Tooltip("����� ������ ����")]
    [SerializeField]
    private List<DropItemSet> dropItemSets = new List<DropItemSet>();

    [Tooltip("�ڿ� ü��")]
    [SerializeField]
    private int resourceHealth = 5;
    [SerializeField]
    private Image healthBar;    //ü�� ������
    private GameObject healthBarBG; //ü�� ������ ���

    [SerializeField]
    [Range(0, 5)]
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int currentHealth; // ���� ü��

    void Start()
    {
        healthBar.fillAmount = 1;
        healthBarBG = healthBar.transform.parent.gameObject;
        healthBarBG.SetActive(false);
    }
    /// <summary>
    /// ������ �� ü�� �ʱ�ȭ
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = resourceHealth;
    }

    /// <summary>
    /// Ŭ���̾�Ʈ�� �ڿ��� ������ �� ȣ�� (�÷��̾� ��ũ��Ʈ���� ȣ��)
    /// </summary>
    [Command(requiresAuthority = false)] // �ƹ� Ŭ���̾�Ʈ�� ȣ�� ����
    public void CmdHitResource()
    {
        // �̹� �ı��� �ڿ��̸� ����
        if (currentHealth <= 0)
        {
            return;
        }

        // ü�� ����
        currentHealth--;

        // �ı��ƴ��� Ȯ��
        if (currentHealth <= 0)
        {
            // ������ ���
            DropItem();

            // �ڿ� ������Ʈ �ı�
            NetworkServer.Destroy(gameObject);
        }
    }

    /// <summary>
    /// ü�� ���� �� ȣ��Ǵ� Hook �Լ�
    /// </summary>
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // ü�¿� ���� �ð��� ��ȭ ����
        UpdateHealthState(newHealth);
    }

    /// <summary>
    /// ü�¹� ����
    /// </summary>
    private void UpdateHealthState(int health)
    {
        healthBarBG?.SetActive(health < resourceHealth);
        healthBar.fillAmount = (float)health / resourceHealth;
    }

    /// <summary>
    /// �������� ������ ���
    /// </summary>
    [Server]
    private void DropItem()
    {
        if (!NetworkServer.active || dropItemPrefab == null)
        {
            return;
        }
        foreach (var dropItemSet in dropItemSets)
        {
            // ������ ������ ����
            for (int i = 0; i < dropItemSet.itemCount; i++)
            {
                // ������ ����
                GameObject dropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
                // ������ �� ����
                NetworkItem item = dropItem.GetComponent<NetworkItem>();
                if (item != null)
                {
                    item.SetItemInfo(dropItemSet.itemID); // ������ �ڵ� ����
                }
                // �������� Ŭ���̾�Ʈ��ο��� ����
                NetworkServer.Spawn(dropItem);
                // ������ ��� ȿ�� ����
                ItemDropMovement dropMovement = dropItem.GetComponent<ItemDropMovement>();
                if (dropMovement != null)
                {
                    dropMovement.InitializePosition(transform.position);
                }
            }
        }
    }
    /// <summary>
    /// �÷��̾ �ڿ��� ��������, Axe �±� ��
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AXE"))
        {
            CmdHitResource();
        }
    }

    /// <summary>
    /// ����� ������ ��Ʈ Ŭ����
    /// </summary>
    [Serializable]
    public class DropItemSet
    {
        public string itemID; // ������ �ڵ�
        public int itemCount; // ����� ������ ����
    }
}