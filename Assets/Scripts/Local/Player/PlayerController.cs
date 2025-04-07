using UnityEngine;
using Mirror;
using TMPro;
using Steamworks;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SyncVar(hook = nameof(OnDisplayNameChanged))]
    private string displayName;

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] float m_climbSpeed = 4.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    [SyncVar(hook = nameof(OnFacingDirectionChanged))]
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private float vertical;
    private PlayerColliderController playerColliderController;

    [Header("몬스터 체력")]

    [Tooltip("최대 체력")]
    [SerializeField]
    private int fullHealth = 100;
    [Tooltip("최소 데미지")]
    [SerializeField]
    private int minDamage = 1;

    [SyncVar(hook = nameof(OnHealthChanged))]
    [Tooltip("현재 체력")]
    [SerializeField]
    private int currentHealth;

    [Tooltip("체력바 이미지")]
    [SerializeField]
    private Image healthBar;
    private GameObject healthBarBG; // 체력바 배경

    [Header("도구 사용")]
    public GameObject attackPoint;  //공격 범위 판정용 오브젝트 
    //인벤토리 접근용 InventoryManager
    //private bool isEquipped = false; //손에 장비 장착 여부

    [Header("자동 체력 감소 설정")]
    [Tooltip("최대 체력이 모두 닳는 데 걸리는 시간(초)")]
    [SerializeField] private float timeToDie = 100f;

    private Coroutine healthDecayCoroutine;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer && SteamManager.Initialized)
        {
            // 내 이름을 가져와서 서버에 설정
            string myName = SteamFriends.GetPersonaName();
            CmdSetDisplayName(myName);
            // 내 카메라만 꺼주기
            virtualCamera.gameObject.SetActive(true);
            //렌더러 우선순위 +1
            GetComponent<SpriteRenderer>().sortingOrder += 1;
        }
        playerColliderController = transform.Find("PlayerCollider").GetComponent<PlayerColliderController>();

        transform.position = new Vector3(Random.Range(-10, 10), 1, 0);

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        attackPoint = transform.Find("AttackPoint").gameObject;
        //attackPoint.SetActive(false); //아이템 Use()에서 Collider2D 컴포넌트를 끄고 키는중 

        currentHealth = fullHealth; // 시작 시 체력을 최대 체력으로 설정

        healthBarBG = healthBar.transform.parent.gameObject;

        healthBar.fillAmount = 1; // 체력바 초기화
        if (healthBar != null)
        {
            healthBarBG.SetActive(false); // 시작 시 체력바 비활성화
        }

        //자동 체력감소 코루틴 시작
        if (isServer)
        {
            healthDecayCoroutine = StartCoroutine(HealthDecayCoroutine());
        }
    }

    /// <summary>
    /// 자동 체력 감소 코루틴
    /// </summary>
    private IEnumerator HealthDecayCoroutine()
    {
        float damagePerSecond = fullHealth / timeToDie;
        WaitForSeconds wait = new WaitForSeconds(10f);

        while (true)
        {
            if (currentHealth > 0)
            {
                CmdHitResource(damagePerSecond);
            }
            yield return wait;
        }
    }

    /// <summary>
    /// 임시 피해 입기 코드
    /// </summary>
    private void TestHit()
    {
        if (m_animator != null)
        {
            m_animator.SetTrigger("Hurt");
        }

        StopCoroutine(ShowHealthBar()); // 체력바 표시

        CmdHitResource(10f); // 피해량 전달

    }

    /// <summary>
    /// 서버에 이름을 설정하도록 요청하는 Command
    /// </summary>
    [Command]
    private void CmdSetDisplayName(string myName)
    {
        // 서버에서 이름 설정 (SyncVar를 통해 모든 클라이언트에 전파됨)
        displayName = myName;
    }

    /// <summary>
    /// 이름이 변경될 때 호출되는 Hook 함수
    /// </summary>
    private void OnDisplayNameChanged(string oldName, string newName)
    {
        playerName.text = newName;
    }

    /// <summary>
    /// 서버에 플레이어가 바라보는 방향을 설정하도록 요청하는 Command
    /// </summary>
    /// <param name="direction"></param>
    [Command]
    private void CmdSetFacingDirection(int direction)
    {
        m_facingDirection = direction;
    }

    /// <summary>
    /// 방향이 변경될 때 호출되는 Hook 함수
    /// </summary>
    private void OnFacingDirectionChanged(int oldDirection, int newDirection)
    {
        GetComponent<SpriteRenderer>().flipX = newDirection == -1;
    }

    /// <summary>
    /// 체력 변경 시 호출되는 메서드 - 체력바 업데이트
    /// </summary>
    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        currentHealth = newHealth;

        // 체력바 업데이트
        if (healthBar != null && currentHealth < fullHealth)
        {
            healthBarBG.SetActive(true); // 체력바 활성화
            healthBar.fillAmount = (float)currentHealth / fullHealth; // 체력 비율 계산
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            CmdSetFacingDirection(m_facingDirection);
            //flipX of attackPoint
            attackPoint.transform.localPosition = new Vector3(1.0f, 0.7f, 0.0f);
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            CmdSetFacingDirection(m_facingDirection);
            //flipX of attackPoint
            attackPoint.transform.localPosition = new Vector3(-1.0f, 0.7f, 0.0f);
        }

        // Move
        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        //장비 장착중인지 검사 > 장비 아이템이라면 장비 아이템의 Use호출
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            if (IsHandEquipped())  // 손에 장비 장착 여부 확인
            {
                InventoryManager.Instance.slots[0].inventoryItem?.Use(this);  // 장비의 Use() 호출


                m_currentAttack++;
                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }


        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
        //Climb
        if (playerColliderController.OverlappingLadderCount > 0)
        {
            vertical = Input.GetAxisRaw("Vertical"); // W, S 또는 ↑, ↓ 키 감지
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            TestHit();
        }
    }
    void FixedUpdate()
    {
        if (playerColliderController.OverlappingLadderCount > 0)
        {
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, vertical * m_climbSpeed);
            m_body2d.gravityScale = 0f; // 중력 제거 (사다리에서 부드럽게 이동)
        }
        else
        {
            m_body2d.gravityScale = 1f; // 다시 원래 중력 복구
        }
    }

    /// <summary>
    /// 장착된 장비 확인
    /// </summary>
    private bool IsHandEquipped()
    {
        var item = InventoryManager.Instance.slots[0].inventoryItem;
        return item != null && item.equipable == Equipable.Hand;
    }


    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    /// <summary>
    /// ???? ???? ???? ?????? ??? : ??? ????????? ???
    /// </summary>
    public Item GetEquippedItem()
    {
        Slot handSlot = InventoryManager.Instance.slots[0];
        if (handSlot != null && handSlot.inventoryItem != null &&
            handSlot.inventoryItem.equipable == Equipable.Hand)
        {
            return handSlot.inventoryItem;
        }

        return null;
    }
    /// <summary>
    /// 아이템을 던집니다. 던진 아이템은 바닥에 떨어지며 바닥에 떨어진 아이템 동기화됨
    /// 인벤토리에서 제거하는 건 InventoryManager에서 처리
    /// </summary>
    [Command (requiresAuthority = false)]
    public void ThrowItem(string itemCode)
    {
        // 아이템 드롭
        NetworkItemManager.Instance.SpawnItem(itemCode, transform.position, false);
    }

    /// <summary>
    /// 임시 공격 판정
    /// </summary>
    /*
    private IEnumerator AttackPointEnable()
    {
        attackPoint.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackPoint.SetActive(false);
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            overlappingLadderCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            overlappingLadderCount--;
        }
    }

    /// <summary>
    /// 피격 함수
    /// </summary>
    [Command(requiresAuthority = false)] // 아무 클라이언트나 호출 가능
    public void CmdHitResource(float damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        // 피해 적용 (반올림 또는 강제 정수 처리)
        currentHealth -= Mathf.CeilToInt(damage);

        // 죽었는지 확인
        if (currentHealth <= 0)
        {
            //TODO : 죽음 처리

            //일단 체력 최대로 올려주는 스크립트
            currentHealth = fullHealth;
            //체력바 초기화
            healthBar.fillAmount = 1;
            if (healthBar != null)
            {
                healthBarBG.SetActive(false); // 시작 시 체력바 비활성화
            }
        }
    }

    /// <summary>
    /// 회복 함수
    /// </summary>

    [Command(requiresAuthority = false)]
    public void CmdHeal(float amount)
    {
        if (currentHealth <= 0 || currentHealth >= fullHealth)
            return;

        currentHealth += Mathf.CeilToInt(amount);
        currentHealth = Mathf.Min(currentHealth, fullHealth); // 최대 체력 제한

    }

    private IEnumerator ShowHealthBar()
    {
        healthBarBG.SetActive(true); // 체력바 활성화
        yield return new WaitForSeconds(2f); // 2초 대기
        healthBarBG.SetActive(false); // 체력바 비활성화
    }
}