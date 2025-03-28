using Steamworks;
using System;
using UnityEngine;

public class PlayerController_Hh : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("���� üũ")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("�κ��丮")]
    public Action inventory;
    public GameObject inventoryWindow;
    public Item itemData;
    public Action addItem;

    [Header("���� ���")]
    public GameObject attackPoint; // ���� ���� ������ ������Ʈ
    public UIInventory uiInventory; // �κ��丮 ���� (0�� ���� ������ ���ٿ�)
    private bool isEquipped = false; // �տ� ��� ���� ����

    private Rigidbody2D rb;
    private bool isGrounded;

    [HideInInspector]
    public bool canLook = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleInventoryKey();
        HandleAttack();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // ���� ��ȯ + attackPoint ��ġ ����
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (attackPoint != null)
                attackPoint.transform.localPosition = new Vector3(1.0f, 0.7f, 0.0f);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if (attackPoint != null)
                attackPoint.transform.localPosition = new Vector3(-1.0f, 0.7f, 0.0f);
        }
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void HandleInventoryKey()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory?.Invoke();
        }
    }

    // ��Ŭ�� �Է����� ���� ���
    private void HandleAttack()
    {
        HandCheck();

        if (Input.GetMouseButtonDown(0) && isEquipped && uiInventory != null)
        {
            Slot handSlot = uiInventory.slots[0];
            if (handSlot != null && handSlot.item != null)
            {
                handSlot.item.Use();
            }
        }
    }

    // 0�� ���� �������� Equipable.Hand���� Ȯ��
    private void HandCheck()
    {
        if (uiInventory == null || uiInventory.slots.Length == 0)
        {
            isEquipped = false;
            return;
        }

        Slot handSlot = uiInventory.slots[0];
        isEquipped = (handSlot != null && handSlot.item != null && handSlot.item.equipable == Equipable.Hand);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            itemData = other.GetComponent<Item>();

            if (itemData != null && inventory != null)
            {
                addItem?.Invoke();
                Destroy(other.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }
}
