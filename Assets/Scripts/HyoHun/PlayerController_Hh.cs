using Steamworks;
using System;
using UnityEngine;

public class PlayerController_Hh : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;         // 좌우 이동 속도
    public float jumpForce = 5f;         // 점프 힘

    [Header("지면 체크")]
    public Transform groundCheck;        // 지면 체크용 위치 기준점
    public float groundCheckRadius = 0.1f; // 지면 체크 반경
    public LayerMask groundLayer;        // 지면으로 인식할 레이어

    [Header("인벤토리")]
    public Action inventory;             // 인벤토리 UI 토글 액션 (E 키로 호출됨)
    public GameObject inventoryWindow;   // 인벤토리 UI 창 직접 제어용 (선택적)
    public Item itemData;                // 충돌한 아이템 데이터
    public Action addItem;               // 아이템을 인벤토리에 추가하는 액션

    private Rigidbody2D rb;              // Rigidbody2D 캐시
    private bool isGrounded;             // 현재 지면에 있는지 여부

    [HideInInspector]
    public bool canLook = true;          // 플레이어가 방향을 바꿀 수 있는지 여부

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
    }

    void Update()
    {
        HandleMovement();      // 좌우 이동
        HandleJump();          // 점프
        HandleInventoryKey();  // 인벤토리 열기 키 입력
    }

    /// <summary>
    /// 좌우 이동 처리 및 방향 전환
    /// </summary>
    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 방향 전환 (왼쪽/오른쪽)
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    /// <summary>
    /// 점프 입력 처리
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
    /// 인벤토리 열기/닫기 키 처리 (E 키)
    /// </summary>
    private void HandleInventoryKey()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory?.Invoke();
        }
    }

    /// <summary>
    /// 아이템 태그를 가진 오브젝트와 충돌 시 아이템 데이터를 저장하고 인벤토리에 추가
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            itemData = other.GetComponent<Item>();

            if (itemData != null && inventory != null)
            {
                addItem?.Invoke();
                Destroy(other.gameObject); // 획득 후 아이템 제거
            }
        }
    }

    /// <summary>
    /// 에디터에서 지면 체크 영역 시각화
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
    /// 인벤토리 UI를 직접 켜거나 끄는 함수
    /// </summary>
    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());
    }

    /// <summary>
    /// 인벤토리 UI가 현재 열려있는지 여부 반환
    /// </summary>
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }
}