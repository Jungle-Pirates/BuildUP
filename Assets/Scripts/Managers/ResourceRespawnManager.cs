using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자원 및 몬스터 리스폰을 담당하는 매니저 클래스
/// </summary>
public class ResourceRespawnManager : Singleton<ResourceRespawnManager>
{
    [Header("리스폰 시간 설정 (초)")]
    [SerializeField] private float respawnDelayMin = 5f;
    [SerializeField] private float respawnDelayMax = 6f;

    [Header("몬스터 리스폰 시간 설정 (초)")]
    [SerializeField] private float monsterRespawnMin = 5f;
    [SerializeField] private float monsterRespawnMax = 6f;

    /// <summary>
    /// 자원 객체의 리스폰을 요청받는 함수
    /// </summary>
    public void RequestRespawn(GameObject resourceObject)
    {
        StartCoroutine(RespawnRoutine(resourceObject));
    }

    /// <summary>
    /// 몬스터 리스폰 요청
    /// </summary>
    public void RequestMonsterRespawn(GameObject monsterObject)
    {
        StartCoroutine(RespawnMonsterRoutine(monsterObject));
    }

    /// <summary>
    /// 일정 시간 후 자원 오브젝트를 다시 활성화시키는 코루틴
    /// </summary>
    private IEnumerator RespawnRoutine(GameObject obj)
    {
        float delay = Random.Range(respawnDelayMin, respawnDelayMax);
        yield return new WaitForSeconds(delay);

        NetworkResource resource = obj.GetComponent<NetworkResource>();
        if (resource != null)
        {
            resource.ResetResource();
            resource.SetActiveState(true); // 서버에서 isActive 값을 true로 설정
        }
    }

    /// <summary>
    /// 몬스터 리스폰 루틴
    /// </summary>
    private IEnumerator RespawnMonsterRoutine(GameObject obj)
    {
        float delay = Random.Range(monsterRespawnMin, monsterRespawnMax);
        yield return new WaitForSeconds(delay);

        obj.SetActive(true); // 반드시 먼저 활성화해야 함

        if (obj.TryGetComponent(out MonsterController monster))
        {
            monster.ResetMonster(); //체력과 상태 초기화
        }
    }
}
