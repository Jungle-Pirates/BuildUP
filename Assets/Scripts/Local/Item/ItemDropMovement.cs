using UnityEngine;
using Mirror;
using DG.Tweening;

public class ItemDropMovement : NetworkBehaviour
{
    [Header("아이템 드롭 설정")]
    [Tooltip("아이템이 퍼지는 반경")]
    [SerializeField] private float spreadRadius = 1.0f;
    [Tooltip("바닥 위 높이 오프셋")]
    [SerializeField] private float heightOffset = 0.1f;
    [Tooltip("드롭 시작 높이")]
    [SerializeField] private float dropHeight = 1.0f;
    [Tooltip("떨어지는 시간")]
    [SerializeField] private float dropDuration = 0.5f;
    [Tooltip("지형 레이어")]
    [SerializeField] private LayerMask groundLayer;

    [SyncVar]
    private Vector3 targetPosition;

    [SyncVar(hook = nameof(OnPositionSet))]
    private bool isPositionSet = false;

    /// <summary>
    /// 포지션이 설정됐을 때 호출되는 콜백
    /// </summary>
    private void OnPositionSet(bool oldPos, bool newPos)
    {
        if (isPositionSet)
        {
            PlayDropAnimation();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 클라이언트 시작 시 아이템이 늦게 생성되어 포지션이 이미 설정되어 있다면 애니메이션 재생
        if (isPositionSet)
        {
            PlayDropAnimation();
        }
    }

    /// <summary>
    /// 서버에서 아이템 위치 초기화
    /// </summary>
    /// <param name="sourcePosition">아이템 스폰되는 위치</param>
    /// <param name="isDrop">랜덤으로 흩뿌려져야 하면 true, 지정된 장소에 떨어져야 하면 false </param>
    [Server]
    public void InitializePosition(Vector2 sourcePosition, bool isDrop)
    {
        Vector2 targetPos = sourcePosition;
        // 드롭 위치를 랜덤으로 흩뿌려지게 설정할지 여부에 따라 다르게 처리
        if (isDrop)
        {
            // X축으로만 랜덤 오프셋 (사이드뷰이므로)
            float randomX = Random.Range(-spreadRadius, spreadRadius);

            // 타겟 위치 계산 (X 오프셋만 적용)
            targetPos = new Vector2(sourcePosition.x + randomX, sourcePosition.y);
        }

        // 바닥 높이 찾기 (2D 레이캐스트 사용)
        float groundY = FindGroundHeight(targetPos);
        // 바닥 높이 + 물건콜라이더길이/2 + 오프셋 적용
        targetPos.y = groundY + (GetComponent<Collider2D>().bounds.size.y / 2) + heightOffset;

        // 목표 위치 설정
        targetPosition = targetPos;
        isPositionSet = true;
    }

    /// <summary>
    /// 바닥 높이 계산 (레이캐스트 사용)
    /// </summary>
    private float FindGroundHeight(Vector2 position)
    {
        // 위에서 아래로 레이캐스트
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(position.x, position.y), // 시작 위치
            Vector2.down, // 아래 방향
            20f, // 최대 거리
            groundLayer // 지형 레이어
        );

        if (hit.collider != null)
        {
            // 지형 높이 반환
            return hit.point.y;
        }

        // 지형을 찾지 못하면 현재 Y값 유지
        return position.y;
    }

    /// <summary>
    /// 드롭 애니메이션
    /// </summary>
    private void PlayDropAnimation()
    {
        // 시작 위치가 타겟 위치보다 높도록 설정
        Vector3 startPos = targetPosition + Vector3.up * dropHeight;
        transform.position = startPos;

        // DOTween으로 타겟 위치까지 이동
        transform.DOMove(targetPosition, dropDuration)
            .SetEase(Ease.OutBounce); // 바운스 효과로 떨어지는 느낌 추가
    }

    /// <summary>
    /// 오브젝트 파괴 시 DOTween KILL 킬 뎀 올!!!!!!!!!!!
    /// </summary>
    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}