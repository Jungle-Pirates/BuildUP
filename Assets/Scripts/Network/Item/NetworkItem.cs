using UnityEngine;
using Mirror;

/// <summary>
/// ���� ������ Ŭ����
/// ����, ��, ����...
/// </summary>

public class NetworkItem : NetworkBehaviour
{
    [Header("������ ����")]
    [Tooltip("������ �ڵ�")]
    public string itemID;

    [SyncVar]
    private bool isPickedUp = false; // ������ ȹ�� ����
    [SyncVar]
    private bool isAbleToPickUp = false; // ������ ȹ�� ���� ����
    private Collider2D itemCollider; // ������ �ݶ��̴�

    void Start()
    {
        itemCollider = GetComponent<Collider2D>();
        itemCollider.enabled = false;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke("MakeItemPickable", 1.0f);
    }

    private void MakeItemPickable()
    {
        isAbleToPickUp = true;
        itemCollider.enabled = true;
    }

    public void SetItemInfo(string id)
    {
        itemID = id;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Items/" + id);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ���������� ó�� & �̹� ȹ��� �������� ����
        if (!isServer || isPickedUp || !isAbleToPickUp) return;

        // �÷��̾�� �浹�ߴ��� Ȯ��
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // �÷��̾ ������ ȹ��
            Debug.Log("�÷��̾ �������� ȹ���߽��ϴ�: " + player.name);

            // ������ ȹ�� ó��
            PlayerPickedUpItem(player);
        }
    }

    /// <summary>
    /// �������� ������ ȹ�� ó��
    /// </summary>
    /// <param name="player">�������� ȹ���� �÷��̾�</param>
    [Server]
    private void PlayerPickedUpItem(PlayerController player)
    {
        // ȹ�� ���·� ����
        isPickedUp = true;

        // �������� �ֿ� �÷��̾�� ������ ȹ�� �˸� �� ������ �κ��丮�� �߰�
        TargetOnItemPickedUp(player.connectionToClient, itemID, 1);

        // ������ ȹ�� ȿ���� ��� Ŭ���̾�Ʈ�� �˸�
        RpcOnItemPickedUp();
    }

    /// <summary>
    /// ��� Ŭ���̾�Ʈ���� ������ ȹ�� ȿ�� ǥ�� �� ����
    /// </summary>
    [ClientRpc]
    private void RpcOnItemPickedUp()
    {
        // ������ ȹ�� ȿ�� ǥ��
        // ...

        //������ ����
        DestroyItem();
    }

    /// <summary>
    /// �������� �ֿ� �÷��̾�� ������ ȹ�� �˸� �� ������ �κ��丮�� �߰�
    /// �������� ȹ���� Ŭ���̾�Ʈ������ �����
    /// </summary>
    [TargetRpc]
    private void TargetOnItemPickedUp(NetworkConnection target, string id, int count)
    {        
        // �κ��丮 �Ŵ����� ���� ������ �߰�
        InventoryManager playerInventory = InventoryManager.Instance;
        if (playerInventory != null)
        {
            // �κ��丮�� ������ �߰�
            playerInventory.AddItem(id, count);
        }
        else
        {
            Debug.LogError("�κ��丮 ���� �Ұ�");
        }
    }

    // ������ ����
    [Server]
    private void DestroyItem()
    {
        // �������� ������ ���� (��� Ŭ���̾�Ʈ�� ����ȭ��)
        NetworkServer.Destroy(gameObject);
    }
}