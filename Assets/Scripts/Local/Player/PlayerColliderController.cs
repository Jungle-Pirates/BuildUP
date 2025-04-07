using Mirror;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    private int overlappingLadderCount = 0; // 플레이어가 사다리와 겹친횟수 0보다 크면 사다리 위에 있음
    public int OverlappingLadderCount => overlappingLadderCount; // 외부에서 접근할 수 있도록 프로퍼티로 제공
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //내 캐릭터가 아니면 무시
        if (GetComponentInParent<NetworkIdentity>().isLocalPlayer == false)
        {
            return;
        }
        if (collision.CompareTag("Monster"))
        {
            // 공격 판정
            //collision.GetComponent<Enemy>().TakeDamage(10);
            //StartCoroutine(AttackPointEnable());
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