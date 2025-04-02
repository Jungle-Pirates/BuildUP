using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
public enum ResourceType
{
    Tree,
    Rock,
    IronVein,
    GoldVein,
    DiamondVein
}
/// <summary>
/// 자원과 몬스터를 생성, 삭제를 관리하는 클래스
/// </summary>
public class NetworkResourceManager : Singleton<NetworkResourceManager>
{
    [Serializable]
    private struct MonsterSet
    {
        public MonsterType monsterType; // 몬스터 타입
        public GameObject monsterPrefab; // 몬스터 프리팹
    }

    [Serializable]
    private struct ResourceSet
    {
        public ResourceType resourceType; // 자원 ID
        public GameObject resourcePrefab; // 자원 프리팹
    }

    [Header("생성될 프리팹")]
    [Tooltip("자원 프리팹 세트")]
    [SerializeField]
    private List<ResourceSet> resourceSets = new List<ResourceSet>();
    [Tooltip("몬스터 프리팹 세트")]
    [SerializeField]
    private List<MonsterSet> monsterSets = new List<MonsterSet>();

    /// <summary>
    /// 월드에 자원을 생성하고, 모든 클라이언트에게도 생성합니다.
    /// </summary>
    /// <param name="position">생성할 위치</param>
    [Server]
    public void SpawnResource(ResourceType resourceType, Vector3 position)
    {
        // 자원 타입에 맞는 프리팹을 찾습니다.
        GameObject resourcePrefab = resourceSets.Find(x => x.resourceType == resourceType).resourcePrefab;
        // 스폰위치를 계산
        var spawnPos = GetGroundSpawnPosition(position, resourcePrefab);
        GameObject resource = Instantiate(resourcePrefab, spawnPos, Quaternion.identity);

        if (resource == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>자원 타입에 맞는 프리팹이 없습니다.");
            return;
        }
        NetworkServer.Spawn(resource);
    }

    /// <summary>
    /// 월드에 몬스터를 생성하고, 모든 클라이언트에게도 생성합니다.
    /// </summary>
    /// <param name="position">생성할 위치</param>
    /// <param name="monsterType">몬스터 타입</param>
    [Server]
    public void SpawnMonster(MonsterType monsterType, Vector3 position)
    {
        // 몬스터 타입에 맞는 프리팹을 찾습니다.
        GameObject monsterPrefab = monsterSets.Find(x => x.monsterType == monsterType).monsterPrefab;
        // 스폰위치를 계산합니다.
        var spawnPos = GetGroundSpawnPosition(position, monsterPrefab);
        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

        if (monster == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>몬스터 타입에 맞는 프리팹이 없습니다.");
            return;
        }
        NetworkServer.Spawn(monster);
    }
    /// <summary>
    /// 자원을 파괴합니다. 각 클라이언트들에게도 자원을 파괴시켜줍니다.
    /// </summary>
    public void DestroyResource(GameObject resource)
    {
        //아이템 파괴와 동일하지만 추가 기능을 넣을때를 대비해 분리함
        if (resource == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>파괴할 자원이 null입니다.");
            return;
        }
        NetworkServer.Destroy(resource);
    }
    /// <summary>
    /// 몬스터를 파괴합니다. 각 클라이언트들에게도 몬스터를 파괴시켜줍니다.
    /// </summary>
    public void DestroyMonster(GameObject monster)
    {
        //아이템 파괴와 동일하지만 추가 기능을 넣을때를 대비해 분리함
        if (monster == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>파괴할 몬스터가 null입니다.");
            return;
        }
        NetworkServer.Destroy(monster);
    }
    /// <summary>
    /// 오브젝트를 스폰할 위치를 계산
    /// </summary>
    /// <param name="position">시작 위치</param>
    /// <param name="objectToSpawn">스폰할 오브젝트</param>
    /// <returns>지면 위에 오브젝트를 배치할 위치, 지면을 찾지 못하면 원래 위치 반환</returns>
    private Vector3 GetGroundSpawnPosition(Vector3 position, GameObject objectToSpawn)
    {
        // 레이캐스트 길이 고정값
        float raycastDistance = 20f;

        // 스폰할 오브젝트의 박스 콜라이더 가져오기
        BoxCollider2D boxCollider = objectToSpawn.GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogWarning("오브젝트에 BoxCollider2D가 없습니다!");
            return position;
        }

        // 박스 콜라이더 기준 오프셋 계산
        float yOffset = (boxCollider.size.y / 2 + boxCollider.offset.y) * objectToSpawn.transform.localScale.y;

        // Ground 레이어 마스크 생성
        int groundLayerMask = 1 << LayerMask.NameToLayer("Ground");

        // 아래 방향으로 레이캐스트 발사
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, raycastDistance, groundLayerMask);

        if (hit.collider != null)
        {
            // 레이캐스트 히트 지점 + 콜라이더 오프셋 적용
            Vector3 groundedPosition = hit.point;
            groundedPosition.y += yOffset;
            Debug.DrawLine(position, groundedPosition, Color.red, 2f); // 디버그용 레이 표시

            return groundedPosition;
        }
        else
        {
            // 지면을 찾지 못한 경우 원래 위치 반환
            Debug.LogWarning("지면을 찾지 못했습니다. 원래 위치를 반환합니다.");
            return position;
        }
    }
}