using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentialRoom : MonoBehaviour
{
    // 이 주거지가 제공하는 일꾼 수
    public int providedWorkers = 1;

    // 주거지가 속한 클러스터 아이디
    public int clusterId;

    // 주거지가 추가될 때 호출
    public void OnPlaced()
    {
        // 인접 주거지 찾기
        FindAdjacentResidences();

        // 주변 주거지들에게 알림
        NotifyAdjacentResidences();

        // 일꾼 업데이트
        HumanResourcesManager.Instance.UpdateTotalWorkers();
    }

    // 주거지가 제거될 때 호출
    public void OnRemoved()
    {
        // 주변 주거지들에게 알림
        NotifyAdjacentResidences();

        // 일꾼 업데이트
        HumanResourcesManager.Instance.UpdateTotalWorkers();
    }

    // 인접 주거지 찾기
    private void FindAdjacentResidences()
    {
        adjacentResidences.Clear();

        // 상, 하, 좌, 우 방향의 인접 셀 확인
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), // 상
            new Vector2Int(0, -1), // 하
            new Vector2Int(-1, 0), // 좌
            new Vector2Int(1, 0) // 우
        };

        Vector2Int currentPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = currentPos + dir;

            // 게임 로직에 맞게 인접 셀의 주거지 확인 방법 구현
            ResidentialRoom adjRoom = FindResidenceAtPosition(checkPos);
            if (adjRoom != null)
            {
                adjacentResidences.Add(adjRoom);
            }
        }
    }

    // 특정 위치의 주거지 찾기 (구현이 필요함)
    private ResidentialRoom FindResidenceAtPosition(Vector2Int pos)
    {
        // 탑 구조와 맞게 구현 필요
        return null; // 임시 반환값
    }

    // 인접 주거지들에게 변경 사항 알림
    private void NotifyAdjacentResidences()
    {
        foreach (ResidentialRoom room in adjacentResidences)
        {
            room.FindAdjacentResidences();
        }
    }

    // 인접 주거지 수에 따른 보너스 일꾼 계산
    public int CalculateBonusWorkers()
    {
        int adjacentCount = adjacentResidences.Count;

        // 인접 주거지 개수에 따른 보너스 계산
        // 5, 9, 12, 14, 15개 일 때마다 보너스 1 추가
        int bonus = 0;
        if (adjacentCount >= 15) bonus = 5;
        else if (adjacentCount >= 14) bonus = 4;
        else if (adjacentCount >= 12) bonus = 3;
        else if (adjacentCount >= 9) bonus = 2;
        else if (adjacentCount >= 5) bonus = 1;

        bonusWorkers = bonus;
        return bonus;
    }

    // 이 주거지가 제공하는 총 일꾼 수 계산
    public int GetTotalProvidedWorkers()
    {
        return providedWorkers + CalculateBonusWorkers();
    }
}