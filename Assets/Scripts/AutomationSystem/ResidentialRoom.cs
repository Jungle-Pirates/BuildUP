using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResidentialRoom : MonoBehaviour
{
    // 이 주거지가 제공하는 일꾼 수
    public int providedWorkers = 1;

    public List<ResidentialRoom> adjacentResidences = new List<ResidentialRoom>();

    public Vector2Int coordinate = Vector2Int.zero; // **향후 수정 필요 Room 클래스의 좌표 받아오기**
    
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
        Vector2Int[] adjacentCoordinates = new Vector2Int[]
        {
            coordinate + Vector2Int.up,
            coordinate + Vector2Int.down,
            coordinate + Vector2Int.left,
            coordinate + Vector2Int.right
        };

        foreach (Vector2Int adj in adjacentCoordinates)
        {
            // 게임 로직에 맞게 인접 셀의 주거지 확인 방법 구현
            ResidentialRoom adjRoom = FindResidenceAtPosition(adj);
            if (adjRoom != null)
            {
                adjacentResidences.Add(adjRoom);
            }
        }
    }

    private ResidentialRoom FindResidenceAtPosition(Vector2Int adjCoordinate)
    {
        if (BuildManager.Instance.WorldRoomData.TryGetValue(adjCoordinate, out GameObject room))
        {
            return room.GetComponent<ResidentialRoom>();
        }
        
        return null;
    }

    // 인접 주거지들에게 변경 사항 알림
    private void NotifyAdjacentResidences()
    {
        foreach (ResidentialRoom room in adjacentResidences)
        {
            room.FindAdjacentResidences();
        }
    }
}