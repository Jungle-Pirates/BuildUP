using UnityEngine;
using Mirror;
using DG.Tweening;

public class ItemDropMovement : NetworkBehaviour
{
    [Header("������ ��� ����")]
    [Tooltip("�������� ������ �ݰ�")]
    [SerializeField] private float spreadRadius = 1.0f;
    [Tooltip("�ٴ� �� ���� ������")]
    [SerializeField] private float heightOffset = 0.1f;
    [Tooltip("��� ���� ����")]
    [SerializeField] private float dropHeight = 1.0f;
    [Tooltip("�������� �ð�")]
    [SerializeField] private float dropDuration = 0.5f;
    [Tooltip("���� ���̾�")]
    [SerializeField] private LayerMask groundLayer;

    [SyncVar]
    private Vector3 targetPosition;

    [SyncVar(hook = nameof(OnPositionSet))]
    private bool isPositionSet = false;

    /// <summary>
    /// �������� �������� �� ȣ��Ǵ� �ݹ�
    /// </summary>
    private void OnPositionSet(bool oldPos, bool newPos)
    {
        if (isPositionSet)
        {
            Debug.Log("<color=green>������ ��ġ ������</color>: " + newPos);
            PlayDropAnimation();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Ŭ���̾�Ʈ ���� �� �������� �ʰ� �����Ǿ� �������� �̹� �����Ǿ� �ִٸ� �ִϸ��̼� ���
        if (isPositionSet)
        {
            PlayDropAnimation();
        }
    }

    // �������� ������ ��ġ �ʱ�ȭ
    [Server]
    public void InitializePosition(Vector2 sourcePosition)
    {
        // X�����θ� ���� ������ (���̵���̹Ƿ�)
        float randomX = Random.Range(-spreadRadius, spreadRadius);

        // Ÿ�� ��ġ ��� (X �����¸� ����)
        Vector2 targetPos = new Vector2(sourcePosition.x + randomX, sourcePosition.y);

        // �ٴ� ���� ã�� (2D ����ĳ��Ʈ ���)
        float groundY = FindGroundHeight(targetPos);
        targetPos.y = groundY + heightOffset;

        // ��ǥ ��ġ ����
        targetPosition = targetPos;
        isPositionSet = true;
    }

    /// <summary>
    /// �ٴ� ���� ��� (����ĳ��Ʈ ���)
    /// </summary>
    private float FindGroundHeight(Vector2 position)
    {
        // ������ �Ʒ��� ����ĳ��Ʈ
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(position.x, position.y), // ���� ��ġ
            Vector2.down, // �Ʒ� ����
            20f, // �ִ� �Ÿ�
            groundLayer // ���� ���̾�
        );

        if (hit.collider != null)
        {
            // ���� ���� ��ȯ
            return hit.point.y;
        }

        // ������ ã�� ���ϸ� ���� Y�� ����
        return position.y;
    }

    /// <summary>
    /// ��� �ִϸ��̼�
    /// </summary>
    private void PlayDropAnimation()
    {
        // ���� ��ġ�� Ÿ�� ��ġ���� ������ ����
        Vector3 startPos = targetPosition + Vector3.up * dropHeight;
        transform.position = startPos;

        // DOTween���� Ÿ�� ��ġ���� �̵�
        transform.DOMove(targetPosition, dropDuration)
            .SetEase(Ease.OutBounce); // �ٿ ȿ���� �������� ���� �߰�
    }

    /// <summary>
    /// ������Ʈ �ı� �� DOTween KILL ų �� ��!!!!!!!!!!!
    /// </summary>
    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}