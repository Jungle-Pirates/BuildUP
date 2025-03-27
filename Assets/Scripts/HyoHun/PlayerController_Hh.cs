using Steamworks;
using System;
using UnityEngine;

public class PlayerController_Hh : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;         // �¿� �̵� �ӵ�
    public float jumpForce = 5f;         // ���� ��

    [Header("���� üũ")]
    public Transform groundCheck;        // ���� üũ�� ��ġ ������
    public float groundCheckRadius = 0.1f; // ���� üũ �ݰ�
    public LayerMask groundLayer;        // �������� �ν��� ���̾�

    [Header("�κ��丮")]
    public Action inventory;             // �κ��丮 UI ��� �׼� (E Ű�� ȣ���)
    public GameObject inventoryWindow;   // �κ��丮 UI â ���� ����� (������)
    public Item itemData;                // �浹�� ������ ������
    public Action addItem;               // �������� �κ��丮�� �߰��ϴ� �׼�

    private Rigidbody2D rb;              // Rigidbody2D ĳ��
    private bool isGrounded;             // ���� ���鿡 �ִ��� ����

    [HideInInspector]
    public bool canLook = true;          // �÷��̾ ������ �ٲ� �� �ִ��� ����

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
    }

    void Update()
    {
        HandleMovement();      // �¿� �̵�
        HandleJump();          // ����
        HandleInventoryKey();  // �κ��丮 ���� Ű �Է�
    }

    /// <summary>
    /// �¿� �̵� ó�� �� ���� ��ȯ
    /// </summary>
    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // ���� ��ȯ (����/������)
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    /// <summary>
    /// ���� �Է� ó��
    /// </summary>
    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    /// <summary>
    /// �κ��丮 ����/�ݱ� Ű ó�� (E Ű)
    /// </summary>
    private void HandleInventoryKey()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory?.Invoke();
        }
    }

    /// <summary>
    /// ������ �±׸� ���� ������Ʈ�� �浹 �� ������ �����͸� �����ϰ� �κ��丮�� �߰�
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            itemData = other.GetComponent<Item>();

            if (itemData != null && inventory != null)
            {
                addItem?.Invoke();
                Destroy(other.gameObject); // ȹ�� �� ������ ����
            }
        }
    }

    /// <summary>
    /// �����Ϳ��� ���� üũ ���� �ð�ȭ
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    /// <summary>
    /// �κ��丮 UI�� ���� �Ѱų� ���� �Լ�
    /// </summary>
    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }

    /// <summary>
    /// �κ��丮 UI�� ���� �����ִ��� ���� ��ȯ
    /// </summary>
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }
}