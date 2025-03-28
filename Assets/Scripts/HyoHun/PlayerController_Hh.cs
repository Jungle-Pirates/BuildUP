using Steamworks;
using System;
using UnityEngine;

public class PlayerController_Hh : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("지면 체크")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("인벤토리")]
    public Action inventory;
    public GameObject inventoryWindow;
    public Item itemData;
    public Action addItem;

    [Header("도구 사용")]
    public GameObject attackPoint; // 공격 범위 판정용 오브젝트
    public UIInventory uiInventory; // 인벤토리 참조 (0번 슬롯 아이템 접근용)
    private bool isEquipped = false; // 손에 장비 착용 여부

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

        // 방향 전환 + attackPoint 위치 조정
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

    // 좌클릭 입력으로 도구 사용
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

    // 0번 슬롯 아이템이 Equipable.Hand인지 확인
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
