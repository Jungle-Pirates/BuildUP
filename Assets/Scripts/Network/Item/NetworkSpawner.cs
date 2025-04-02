using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 임시 자원 스포너 클래스
/// </summary>
public class NetworkSpawner : NetworkBehaviour
{
    [Serializable]
    private struct SpawnResourceSet
    {
        public ResourceType resourceType; // 자원 타입
        public int spawnCount; // 스폰할 갯수
    }
    [Serializable]
    private struct SpawnMonsterSet
    {
        public MonsterType monsterType; // 몬스터 타입
        public int spawnCount; // 스폰할 갯수
    }

    [Header("스폰 세팅")]
    [Tooltip("스폰할 지역")]
    [SerializeField]
    private Transform spawnArea; // 스폰할 지역
    private GameObject[] groundObjects; //스폰되면 안되는 땅 지역
    [SerializeField]

    [Space]

    [Tooltip("자원 스폰 갯수")]
    private List<SpawnResourceSet> spawnResourceSets = new List<SpawnResourceSet>();

    [Space]

    [Tooltip("몬스터 스폰 갯수")]
    [SerializeField]
    private List<SpawnMonsterSet> spawnMonsterSets = new List<SpawnMonsterSet>();

    public override void OnStartServer()
    {
        // 서버에서만 실행
        if (!isServer)
        {
            return;
        }
        base.OnStartServer();

        //Ground 레이어 오브젝트를 모두 찾아서 리스트에 추가 (태그는 없음)
        groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        SpawnResources();
        SpawnMonsters();
    }
    [Server]
    private void SpawnResources()
    {
        foreach (var spawnSet in spawnResourceSets)
        {
            for (int i = 0; i < spawnSet.spawnCount; i++)
            {
                Vector3 randomPosition = new Vector3(
                    UnityEngine.Random.Range(spawnArea.position.x - spawnArea.localScale.x / 2, spawnArea.position.x + spawnArea.localScale.x / 2),
                    UnityEngine.Random.Range(spawnArea.position.y - spawnArea.localScale.y / 2, spawnArea.position.y + spawnArea.localScale.y / 2),
                    UnityEngine.Random.Range(spawnArea.position.z - spawnArea.localScale.z / 2, spawnArea.position.z + spawnArea.localScale.z / 2)
                );
                bool isValidPosition = true;
                //Ground 레이어 오브젝트와 겹치면 다시 랜덤 위치 생성
                foreach (var groundObject in groundObjects)
                {
                    if (groundObject.GetComponent<Collider2D>().bounds.Contains(randomPosition))
                    {
                        isValidPosition = false;
                        break;
                    }
                }
                //스폰할 위치가 유효하지 않으면 다시 랜덤 위치 생성
                if (!isValidPosition)
                {
                    i--;
                    continue;
                }
                NetworkResourceManager.Instance.SpawnResource(spawnSet.resourceType, randomPosition);
            }
        }
    }
    [Server]
    private void SpawnMonsters()
    {
        foreach (var spawnSet in spawnMonsterSets)
        {
            for (int i = 0; i < spawnSet.spawnCount; i++)
            {
                Vector3 randomPosition = new Vector3(
                    UnityEngine.Random.Range(spawnArea.position.x - spawnArea.localScale.x / 2, spawnArea.position.x + spawnArea.localScale.x / 2),
                    spawnArea.position.y,
                    UnityEngine.Random.Range(spawnArea.position.z - spawnArea.localScale.z / 2, spawnArea.position.z + spawnArea.localScale.z / 2)
                );
                NetworkResourceManager.Instance.SpawnMonster(spawnSet.monsterType, randomPosition);
            }
        }
    }
}