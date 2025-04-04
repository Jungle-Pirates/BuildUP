using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 몬스터 종류
/// </summary>
public enum MonsterType
{
    Boar, Chicken, Stone
}
/// <summary>
/// 몬스터 컨트롤러
/// </summary>
public class MonsterController : NetworkBehaviour
{
    #region 몬스터 공통 설정
    [Header("몬스터 종류")]

    [SerializeField]
    private MonsterType monsterType; // 몬스터 종류
    [Range(0, 10)]
    [Tooltip("몬스터 속도")]
    [SerializeField]
    private float monsterSpeed;
    [Tooltip("몬스터 공격력")]
    [SerializeField]
    private float monsterDamage;
    public float MonsterDamage => monsterDamage; // 공격력 getter

    [Space]

    [Header("도구 태그")]

    [Tooltip("이 자원에 피해를 줄 수 있는 도구 태그 (예: AXE, PICK, SWORD)")]
    [SerializeField]
    private string toolTag = "SWORD";

    [Space]

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

    [Space]

    [Header("드롭 아이템 정보")]

    [Tooltip("드롭할 아이템 프리팹")]
    [SerializeField]
    private GameObject dropItemPrefab;
    [Tooltip("드롭할 아이템 세트")]
    [SerializeField]
    private List<DropItemSet> dropItemSets = new List<DropItemSet>();


    [Space]

    [Header("움직임 설정")]
    [Tooltip("플랫폼 체크 거리")]
    [SerializeField]
    private float platformCheckDistance = 1.0f;

    [Tooltip("이동 방향 (1: 오른쪽, -1: 왼쪽)")]
    private int moveDirection = 1;

    [Tooltip("스프라이트 방향 뒤집기 상태")]
    [SyncVar(hook = nameof(OnFlipXChanged))]
    private bool isFlipped = true;

    [Tooltip("움직임 상태 (true: 움직임, false: 정지)")]
    private bool isMoving = false;

    [Tooltip("정지 시간 범위")]
    [SerializeField]
    private Vector2 idleTimeRange = new Vector2(1f, 3f);

    [Tooltip("이동 시간 범위")]
    [SerializeField]
    private Vector2 moveTimeRange = new Vector2(2f, 5f);

    #endregion

    [Space]

    #region 몬스터 별 설정

    #region 닭 설정

    [Header("몬스터 - 닭 설정")]

    [Tooltip("닭이 도망가는 시간")]
    [SerializeField]
    private float chickenFleeTime = 5f;

    [Tooltip("도망갈 때 속도 배율")]
    [SerializeField]
    private float fleeSpeedMultiplier = 1.5f;

    #endregion

    [Space]

    #region 멧돼지 설정

    [Header("몬스터 - 멧돼지 설정")]

    [Tooltip("플레이어 감지 범위")]
    [SerializeField]
    private float boarDetectionRange = 5f;

    [Tooltip("돌진 속도 배율")]
    [SerializeField]
    private float boarChargeSpeedMultiplier = 2.5f;

    [Tooltip("돌진 지속 시간")]
    [SerializeField]
    private float boarChargeDuration = 2f;

    [Tooltip("돌진 쿨다운 시간")]
    [SerializeField]
    private float boarChargeCooldown = 5f;
    [Tooltip("돌진 후 휴식 시간")]
    [SerializeField]
    private float boarRestAfterChargeDuration = 3f;


    // 멧돼지 상태 변수들
    private bool isResting = false;
    private float restTimer = 0f;
    private bool isCharging = false;
    private float chargeTimer = 0f;
    private float chargeCooldownTimer = 0f;
    private Transform targetPlayer = null;

    #endregion

    [Space]

    #region 돌 설정

    [Header("몬스터 - 돌 설정")]
    [Tooltip("돌이 도망가는 시간")]
    [SerializeField]
    private float stoneFleeTime = 4f;

    [Tooltip("돌 도망 속도 배율")]
    [SerializeField]
    private float stoneFleeSpeedMultiplier = 1.2f;

    #endregion

    #endregion

    [Header("private 변수들")]
    private Animator animator; // 애니메이터
    private Rigidbody2D rb; // 리지드바디
    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    private Transform targetToFleeFrom; // 도망갈 대상
    private bool isFleeing = false; // 도망가는 중인지
    private float currentActionTime = 0f; // 현재 행동 시간
    private float maxActionTime = 0f; // 최대 행동 시간

    [ClientRpc]
    private void RpcSetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 로컬 데이터 초기화 할때 사용
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBarBG = healthBar.transform.parent.gameObject;

        healthBar.fillAmount = 1; // 체력바 초기화
        if (healthBar != null)
        {
            healthBarBG.SetActive(false); // 시작 시 체력바 비활성화
        }
        if (isServer)
        {
            DecideNextAction(); // 서버에서 몬스터 행동 결정
        }
    }

    /// <summary>
    /// 공유되는 데이터 값 초기화 할때 사용
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = fullHealth; // 시작 시 체력을 최대 체력으로 설정
    }

    #region 체력과 상태관리

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
    /// <summary>
    /// 스프라이트 방향 변경 시 호출 - 플립 상태 동기화 처리
    /// </summary>
    private void OnFlipXChanged(bool oldValue, bool newValue)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !newValue;
        }
    }

    /// <summary>
    /// 클라이언트가 생명체를 때렸을 때 호출 (호스트 클라이언트에서 실행됨)
    /// </summary>
    [Command(requiresAuthority = false)] // 아무 클라이언트나 호출 가능
    public void CmdHitResource(float damage, GameObject attacker)
    {
        // 이미 잡은 몬스터면 무시
        if (currentHealth <= 0)
        {
            return;
        }

        // 피해 적용 (반올림 또는 강제 정수 처리)
        currentHealth -= Mathf.CeilToInt(damage);
        // 맞았을 때 애니메이션 트리거
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // 닭이 공격 당했을 때 도망가기
        if ((monsterType == MonsterType.Chicken || monsterType == MonsterType.Stone) && attacker != null)
        {
            StartFleeingFrom(attacker.transform);
        }
        // 멧돼지가 공격당했을 때 공격자 방향으로 바라보기
        else if (monsterType == MonsterType.Boar)
        {
            FaceAttacker(attacker.transform);
        }

        // 죽었는지 확인
        if (currentHealth <= 0)
        {
            // 아이템 드롭
            DropItem();

            // 서버에서만 처리
            if (isServer)
            {
                // 오브젝트 비활성화 (서버에서 실행되면 클라에도 반영됨)
                RpcSetActive(false);

                // 리스폰 요청
                ResourceRespawnManager.Instance.RequestMonsterRespawn(gameObject);
            }

            // 생명체 오브젝트 파괴
            //NetworkResourceManager.Instance.DestroyMonster(gameObject);
        }
    }

    [Server]
    public void ResetMonster()
    {
        currentHealth = fullHealth;

        healthBar.fillAmount = 1; // 체력바 초기화
        if (healthBar != null)
        {
            healthBarBG.SetActive(false); // 시작 시 체력바 비활성화
        }
        /*
        if (isServer)
        {
            DecideNextAction(); // 서버에서 몬스터 행동 결정
        }
        */
        // 필요하다면 위치도 초기화
        // transform.position = spawnPoint;

        if (animator != null)
        {
            animator.Rebind();  // 애니메이션 초기화
            animator.Update(0f);
        }

        RpcSetActive(true); // 클라이언트들도 활성화
    }

    /// <summary>
    /// 서버에서 아이템 드롭
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
            // 아이템 여러개 생성
            for (int i = 0; i < dropItemSet.itemCount; i++)
            {
                NetworkItemManager.Instance.SpawnItem(dropItemSet.itemID, transform.position, true);
            }
        }
    }
    #endregion

    #region 움직임 및 애니메이션

    void Update()
    {
        // 서버에서만 움직임 로직 처리
        if (isServer && currentHealth > 0)
        {
            switch (monsterType)
            {
                case MonsterType.Chicken:
                    ChickenMovement();
                    break;
                case MonsterType.Boar:
                    BoarMovement();
                    break;
                case MonsterType.Stone:
                    StoneMovement();
                    break;
            }
            if (animator != null)
            {
                animator.SetBool("Move", isMoving);
            }
        }
    }

    /// <summary>
    /// 다음 행동 결정 (이동 또는 정지)
    /// </summary>
    [Server]
    private void DecideNextAction()
    {
        if (isFleeing || isCharging || isResting)
            return;

        switch (monsterType)
        {
            case MonsterType.Chicken:
                // 닭은 70% 확률로 움직임
                isMoving = Random.value < 0.7f;
                break;
            case MonsterType.Boar:
                // 멧돼지는 50% 확률로 움직임
                isMoving = Random.value < 0.5f;
                break;
            case MonsterType.Stone:
                // 스톤은 기본 행동
                isMoving = Random.value < 0.2f;
                break;
            default:
                isMoving = Random.value < 0.5f;
                break;
        }

        // 랜덤하게 방향 결정 (20% 확률로 방향 전환)
        if (Random.value < 0.2f)
        {
            moveDirection = -moveDirection;
            isFlipped = (moveDirection < 0);
        }

        // 행동 시간 설정
        maxActionTime = isMoving ?
            Random.Range(moveTimeRange.x, moveTimeRange.y) :
            Random.Range(idleTimeRange.x, idleTimeRange.y);

        currentActionTime = 0f;

        // 애니메이션 상태 설정
        if (animator != null)
        {
            animator.SetBool("Move", isMoving);
        }
    }
    /// <summary>
    /// 앞에 플랫폼이 있는지 체크
    /// </summary>
    private bool IsPlatformAhead()
    {
        Vector2 rayStart = new Vector2(
            transform.position.x + (moveDirection * 0.5f),
            transform.position.y - 0.1f  // 약간 아래에서 시작
        );

        // 플랫폼 레이어를 대상으로 레이캐스트
        int platformLayer = LayerMask.GetMask("Platform", "Ground", "OneWayPlatform");
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, platformCheckDistance, platformLayer);

        // 디버그 레이 그리기
        Debug.DrawRay(rayStart, Vector2.down * platformCheckDistance, hit ? Color.green : Color.red);

        return hit.collider != null;
    }
    /// <summary>
    /// 공격자 방향으로 바라보기
    /// </summary>
    [Server]
    private void FaceAttacker(Transform attacker)
    {
        if (attacker != null)
        {
            // 공격자가 오른쪽에 있으면 오른쪽을 바라봄
            if (attacker.position.x > transform.position.x)
            {
                moveDirection = 1;
            }
            // 공격자가 왼쪽에 있으면 왼쪽을 바라봄
            else
            {
                moveDirection = -1;
            }

            // 스프라이트 방향 설정
            isFlipped = (moveDirection < 0);
        }
    }

    /// <summary>
    /// 방향 전환
    /// </summary>
    [Server]
    private void FlipDirection()
    {
        moveDirection = -moveDirection;
        isFlipped = (moveDirection > 0);
    }

    /// <summary>
    /// 도망가기 시작
    /// </summary>
    [Server]
    private void StartFleeingFrom(Transform target)
    {
        if (!isFleeing)
        {
            targetToFleeFrom = target;
            isFleeing = true;
            isMoving = true;

            // 플레이어의 반대 방향으로 이동
            if (targetToFleeFrom.position.x > transform.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }

            // 애니메이션 상태 변경
            if (animator != null)
            {
                animator.SetBool("Move", true);
            }
            // 일정 시간 후 도망가기 종료
            float fleeTime = 0;

            switch (monsterType)
            {
                case MonsterType.Chicken:
                    fleeTime = chickenFleeTime;
                    break;
                case MonsterType.Stone:
                    fleeTime = stoneFleeTime;
                    break;
                default:
                    fleeTime = 3f; // 기본값
                    break;
            }
            // 일정 시간 후 도망가기 종료
            StartCoroutine(StopFleeingAfterTime(fleeTime));
        }
    }

    /// <summary>
    /// 일정 시간 후 도망가기 종료
    /// </summary>
    private IEnumerator StopFleeingAfterTime(float fleeTime)
    {
        yield return new WaitForSeconds(fleeTime);
        isFleeing = false;
        DecideNextAction();
    }
    #region 몬스터별 움직임

    /// <summary>
    /// 닭 움직임 로직
    /// </summary>
    [Server]
    private void ChickenMovement()
    {
        currentActionTime += Time.deltaTime;

        // 도망가는 중이면 플레이어의 반대 방향으로 이동
        if (isFleeing && targetToFleeFrom != null)
        {
            // 플레이어로부터 반대 방향으로 이동
            if (targetToFleeFrom.position.x > transform.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }

            float currentSpeed = monsterSpeed * fleeSpeedMultiplier;

            // 앞에 플랫폼이 없으면 방향 전환
            if (!IsPlatformAhead())
            {
                FlipDirection();
            }

            // 이동 처리
            rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);
            isFlipped = (moveDirection < 0);
        }
        // 일반 움직임 처리
        else if (isMoving)
        {
            // 앞에 플랫폼이 없으면 방향 전환
            if (!IsPlatformAhead())
            {
                FlipDirection();
            }

            // 이동 처리
            rb.velocity = new Vector2(moveDirection * monsterSpeed, rb.velocity.y);
            isFlipped = (moveDirection < 0);

            // 행동 시간이 지나면 다음 행동 결정
            if (currentActionTime >= maxActionTime)
            {
                DecideNextAction();
            }
        }
        else
        {
            // 정지 상태
            rb.velocity = new Vector2(0, rb.velocity.y);

            // 행동 시간이 지나면 다음 행동 결정
            if (currentActionTime >= maxActionTime)
            {
                DecideNextAction();
            }
        }
    }

    /// <summary>
    /// 멧돼지 움직임 로직 (아직 미구현)
    /// </summary>
    [Server]
    private void BoarMovement()
    {
        // 휴식 상태 처리
        if (isResting)
        {
            restTimer += Time.deltaTime;

            // 휴식 시간이 끝나면 일반 상태로 전환
            if (restTimer >= boarRestAfterChargeDuration)
            {
                isResting = false;
                chargeCooldownTimer = 0f;
                DecideNextAction();
            }

            return;
        }

        // 돌진 중일 때
        if (isCharging)
        {
            // 돌진 타이머 업데이트
            chargeTimer += Time.deltaTime;

            // 돌진 방향 설정 (처음 돌진 시작 방향으로 유지)
            float chargeSpeed = monsterSpeed * boarChargeSpeedMultiplier;
            rb.velocity = new Vector2(moveDirection * chargeSpeed, rb.velocity.y);

            // 돌진 시간이 끝나면 휴식 상태로 변경
            if (chargeTimer >= boarChargeDuration)
            {
                isCharging = false;
                isMoving = false;
                isResting = true;
                restTimer = 0f;
                animator.SetBool("Move", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            // 앞에 플랫폼이 없으면 돌진 중단하고 휴식 상태로 전환
            if (!IsPlatformAhead())
            {
                isCharging = false;
                isMoving = false;
                isResting = true;
                restTimer = 0f;
                animator.SetBool("Move", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
                FlipDirection();
            }

            return;
        }

        // 쿨다운 타이머 업데이트
        if (chargeCooldownTimer < boarChargeCooldown)
        {
            chargeCooldownTimer += Time.deltaTime;
        }

        // 플레이어 감지 및 돌진 시작 (쿨다운이 끝났을 때만)
        if (chargeCooldownTimer >= boarChargeCooldown)
        {
            // 정면 방향으로만 플레이어 감지
            Vector2 detectionOrigin = transform.position;
            Vector2 detectionSize = new Vector2(boarDetectionRange, 2f); // 너비 5미터, 높이 2미터

            // 검출 위치 조정 (정면 방향으로)
            detectionOrigin.x += (moveDirection * boarDetectionRange / 2);

            // 방향에 따른 박스 위치 조정
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(detectionOrigin, detectionSize, 0f);

            foreach (Collider2D hitCollider in hitColliders)
            {
                // 플레이어 감지
                PlayerController player = hitCollider.GetComponent<PlayerController>();
                if (player != null)
                {
                    // 플레이어가 멧돼지 앞에 있는지 확인 (방향 체크)
                    bool isInFront = (player.transform.position.x - transform.position.x) * moveDirection > 0;

                    if (isInFront)
                    {
                        targetPlayer = player.transform;

                        // 돌진 시작
                        isCharging = true;
                        isMoving = true;
                        chargeTimer = 0f;
                        animator.SetBool("Move", true);

                        // 첫 번째 감지된 플레이어에게만 돌진
                        break;
                    }
                }
            }
        }

        // 일반 움직임 처리 (돌진 중이 아닐 때)
        if (!isCharging)
        {
            currentActionTime += Time.deltaTime;

            if (isMoving)
            {
                // 앞에 플랫폼이 없으면 방향 전환
                if (!IsPlatformAhead())
                {
                    FlipDirection();
                }

                // 이동 처리
                rb.velocity = new Vector2(moveDirection * monsterSpeed, rb.velocity.y);
                isFlipped = (moveDirection < 0);

                // 행동 시간이 지나면 다음 행동 결정
                if (currentActionTime >= maxActionTime)
                {
                    DecideNextAction();
                }
            }
            else
            {
                // 정지 상태
                rb.velocity = new Vector2(0, rb.velocity.y);

                // 애니메이션 상태 설정
                animator.SetBool("Move", false);

                // 행동 시간이 지나면 다음 행동 결정
                if (currentActionTime >= maxActionTime)
                {
                    DecideNextAction();
                }
            }
        }
    }

    /// <summary>
    /// 돌 움직임 로직 (아직 미구현)
    /// </summary>
    [Server]
    private void StoneMovement()
    {
        currentActionTime += Time.deltaTime;

        // 도망가는 중이면 계속 도망가기
        if (isFleeing && targetToFleeFrom != null)
        {
            // 플레이어로부터 반대 방향으로 이동
            if (targetToFleeFrom.position.x > transform.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }

            float currentSpeed = monsterSpeed * stoneFleeSpeedMultiplier;

            // 앞에 플랫폼이 없으면 방향 전환
            if (!IsPlatformAhead())
            {
                FlipDirection();
            }

            // 이동 처리
            rb.velocity = new Vector2(moveDirection * currentSpeed, rb.velocity.y);
            isFlipped = (moveDirection < 0);
        }
        // 일반 움직임 처리
        else if (isMoving)
        {
            // 앞에 플랫폼이 없으면 방향 전환
            if (!IsPlatformAhead())
            {
                FlipDirection();
            }

            // 이동 처리
            rb.velocity = new Vector2(moveDirection * monsterSpeed, rb.velocity.y);
            isFlipped = (moveDirection < 0);

            // 행동 시간이 지나면 다음 행동 결정
            if (currentActionTime >= maxActionTime)
            {
                DecideNextAction();
            }
        }
        else
        {
            // 정지 상태
            rb.velocity = new Vector2(0, rb.velocity.y);

            // 행동 시간이 지나면 다음 행동 결정
            if (currentActionTime >= maxActionTime)
            {
                DecideNextAction();
            }
        }
    }

    #endregion
    #endregion

    #region 피격 판정

    /// <summary>
    /// 플레이어가 자원을 때렸을 때, 도구 태그 비교 및 피해량 전달
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 객체의 루트가 Player인지 확인
        Transform root = collision.transform.root;
        PlayerController player = root.GetComponent<PlayerController>();

        if (player == null)
            return;

        Debug.Log($"충돌한 객체: {collision.gameObject.name}");
        // 플레이어 장비 정보 가져오기
        Item equipped = player.GetEquippedItem();

        // null 체크 + 태그 일치 확인
        if (equipped != null && collision.CompareTag(toolTag))
        {
            // HandyToolItem인지 확인 후 damage 값 가져오기
            HandyToolItem tool = equipped as HandyToolItem;
            if (tool != null)
            {
                StopCoroutine("ShowHealthBar"); // 체력바 표시 중지
                healthBarBG.SetActive(false); // 체력바 비활성화
                if (tool.damage < minDamage)
                {
                    StartCoroutine("ShowHealthBar"); // 체력바 표시
                    return; // 최소 데미지보다 작으면 무시
                }
                CmdHitResource(tool.damage, player.gameObject); // 피해량 전달
            }
        }
    }
    private IEnumerator ShowHealthBar()
    {
        healthBarBG.SetActive(true); // 체력바 활성화
        yield return new WaitForSeconds(2f); // 2초 대기
        healthBarBG.SetActive(false); // 체력바 비활성화
    }

    #endregion
}
