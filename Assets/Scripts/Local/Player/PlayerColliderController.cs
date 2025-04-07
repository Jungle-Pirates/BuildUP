using Mirror;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    private int overlappingLadderCount = 0; // 플레이어가 사다리와 겹친횟수 0보다 크면 사다리 위에 있음
    public int OverlappingLadderCount => overlappingLadderCount; // 외부에서 접근할 수 있도록 프로퍼티로 제공

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //내 캐릭터가 아니면 무시
        if (GetComponentInParent<NetworkIdentity>().isLocalPlayer == false)
        {
            return;
        }
        if (collision.CompareTag("Monster"))
        {
            //맷돼지인지 체크
            if (collision.GetComponent<MonsterController>() is MonsterController monsterCon)
            {
                if (monsterCon.monsterType == MonsterType.Boar)
                {
                    // 맞았을 때 애니메이션 트리거
                    
                    if (playerController.m_animator != null)
                    {
                        playerController.m_animator.SetTrigger("Hurt");
                    }

                    StopCoroutine(playerController.ShowHealthBar()); // 체력바 표시

                    playerController.CmdHitResource(monsterCon.MonsterDamage); // 피해량 전달
                    
                }
            }
        }
        if (collision.CompareTag("Ladder"))
        {
            overlappingLadderCount++;
        }
        if (collision.CompareTag("Room"))
        {
            // 방이 사용중이면 UI를 열지 않음
            if (collision.GetComponent<Room>().IsOccupied && collision.GetComponent<Room>().isOccupiedByMe == false)
            {
                Debug.LogWarning($"방이 사용중입니다: {collision.name}");
                return;
            }
            collision.GetComponent<Room>().OpenRoomUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //내 캐릭터가 아니면 무시
        if (GetComponentInParent<NetworkIdentity>().isLocalPlayer == false)
        {
            return;
        }
        if (collision.CompareTag("Ladder"))
        {
            overlappingLadderCount--;
        }
        if (collision.CompareTag("Room"))
        {
            collision.GetComponent<Room>().CloseRoomUI();
        }
    }
}