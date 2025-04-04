using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanResourcesManager : Singleton<HumanResourcesManager>
{
    // 전체 일꾼 수
    public int totalWorkers = 0;
    
    // 현재 사용 가능한 일꾼 수
    public int availableWorkers = 0;
    
    // 모든 주거지 리스트
    public List<ResidentialRoom> allResidences = new List<ResidentialRoom>();
    
    // 식별된 클러스터 목록
    private List<List<ResidentialRoom>> clusters = new List<List<ResidentialRoom>>();
    
    // 클러스터별 일꾼 총합
    private List<int> clusterTotalWorkers = new List<int>();
    
    // 작업이 할당된 모든 방 리스트
    public List<Room> assignedRooms = new List<Room>();

    public void RecalculateClusters()
    {
        // 클러스터 초기화
        clusters.Clear();
        clusterTotalWorkers.Clear();
        
        // 방문 상태 추적
        HashSet<ResidentialRoom> visited = new HashSet<ResidentialRoom>();
        
        // 모든 주거지에 대해 클러스터링 수행
        foreach (ResidentialRoom room in allResidences)
        {
            if (!visited.Contains(room))
            {
                // 새 클러스터 생성 및 DFS로 연결된 모든 주거지 찾기
                List<ResidentialRoom> newCluster = new List<ResidentialRoom>();
                DiscoverCluster(room, newCluster, visited);
                
                // 클러스터 ID 할당 (리스트 인덱스 사용)
                int clusterId = clusters.Count;
                foreach (ResidentialRoom r in newCluster)
                {
                    r.clusterId = clusterId;
                }
                
                clusters.Add(newCluster);
            }
        }
        
        // 각 클러스터의 보너스 계산
        CalculateClusterBonuses();
        
        // 총 일꾼 수 업데이트
        UpdateTotalWorkers();
    }
   
    // DFS로 연결된 모든 주거지 탐색
    private void DiscoverCluster(ResidentialRoom room, List<ResidentialRoom> cluster, HashSet<ResidentialRoom> visited)
    {
        // 방문 표시 및 클러스터에 추가
        visited.Add(room);
        cluster.Add(room);
        
        // 인접한 모든 주거지에 대해 재귀 탐색
        foreach (ResidentialRoom neighbor in room.adjacentResidences)
        {
            if (!visited.Contains(neighbor))
            {
                DiscoverCluster(neighbor, cluster, visited);
            }
        }
    }
    
    private void CalculateClusterBonuses()
    {
        for (int i = 0; i < clusters.Count; i++)
        {
            List<ResidentialRoom> cluster = clusters[i];
            int clusterSize = cluster.Count;
            
            // 기본 일꾼 수 (각 주거지마다 1명)
            int baseWorkers = clusterSize;
            
            // 클러스터 크기에 따른 보너스 (한 클러스터당 한 번만 적용)
            int bonus = 0;
            if (clusterSize >= 15) bonus = 5;
            else if (clusterSize >= 14) bonus = 4;
            else if (clusterSize >= 12) bonus = 3;
            else if (clusterSize >= 9) bonus = 2;
            else if (clusterSize >= 5) bonus = 1;
            
            // 클러스터 총 일꾼 수
            clusterTotalWorkers[i] = baseWorkers + bonus;
            
            Debug.Log($"주거지 클러스터 {i}: 크기={clusterSize}, 기본 일꾼={baseWorkers}, 보너스={bonus}, 총={totalWorkers}");
        }
    }
    
    public void UpdateTotalWorkers()
    {
        int previousTotal = totalWorkers;
        totalWorkers = 0;

        // 모든 주거지 클러스터에서 제공하는 일꾼 수 계산
        foreach (int workers in clusterTotalWorkers)
        {
            totalWorkers += workers;
        }

        // 새로 획득한/잃은 일꾼 수 계산
        int workerDifference = totalWorkers - previousTotal;

        // 가용 일꾼 수 업데이트
        availableWorkers += workerDifference;

        // 가용 일꾼이 음수가 되면 방에서 일꾼 회수
        if (availableWorkers < 0)
        {
            RecoverWorkersFromRooms(-availableWorkers);
        }

        // UI 업데이트
        UpdateWorkerUI();
    }

    // 방에 일꾼 할당
    public bool AssignWorkersToRoom(Room room, int count)
    {
        if (availableWorkers >= count)
        {
            if (room.AssignWorker(count))
            {
                availableWorkers -= count;
                
                // 이미 리스트에 없다면 추가
                if (!assignedRooms.Contains(room))
                {
                    assignedRooms.Add(room);
                }
                
                UpdateWorkerUI();
                return true;
            }
        }
        return false;
    }
    
    // 방에서 일꾼 회수
    public void ReleaseWorkers(Room room, int count)
    {
        if (room.RemoveWorker(count))
        {
            availableWorkers += count;
            
            // 일꾼이 없어지면 리스트에서 제거
            if (room.CurrentWorkers == 0)
            {
                assignedRooms.Remove(room);
            }
            
            UpdateWorkerUI();
        }
    }
    
    // 일꾼 감소 시 방에서 일꾼 회수 (우선순위에 따라)
    private void RecoverWorkersFromRooms(int count)
    {
        // 우선순위 역순으로 정렬 (낮은 우선순위부터 회수)
        assignedRooms.Sort((a, b) => a.priority.CompareTo(b.priority));
        
        int remainingToRecover = count;
        
        // 필요한 만큼 회수
        for (int i = 0; i < assignedRooms.Count && remainingToRecover > 0; i++)
        {
            Room room = assignedRooms[i];
            int workerToRemove = Mathf.Min(remainingToRecover, room.CurrentWorkers);
            
            if (workerToRemove > 0)
            {
                room.RemoveWorker(workerToRemove);
                remainingToRecover -= workerToRemove;
                
                // 모든 일꾼이 제거되면 리스트에서 제거
                if (room.CurrentWorkers == 0)
                {
                    assignedRooms.Remove(room);
                    i--; // 리스트 변경에 따른 인덱스 조정
                }
            }
        }
        
        // 가용 일꾼 수 0으로 조정 (음수가 되지 않도록)
        availableWorkers = 0;
    }
    
    // 우선순위에 따라 일꾼 재분배
    /*
    public void ReassignWorkersByPriority()
    {
        // 모든 방에서 일꾼 회수
        int totalAssigned = 0;
        foreach (Room room in assignedRooms.ToArray())
        {
            totalAssigned += room.currentWorkers;
            ReleaseWorkers(room, room.currentWorkers);
        }
        
        // 우선순위에 따라 방 정렬 (높은 우선순위부터)
        assignedRooms.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        // 우선순위에 따라 일꾼 재할당
        foreach (Room room in assignedRooms.ToArray())
        {
            if (room.isAutomated && availableWorkers > 0)
            {
                int workersToAssign = Mathf.Min(availableWorkers, room.maxWorkers);
                AssignWorkersToRoom(room, workersToAssign);
            }
        }
        
        UpdateWorkerUI();
    }
    */
    
    // UI 업데이트
    private void UpdateWorkerUI()
    {
        int usedWorkers = 0;
        foreach (Room room in assignedRooms)
        {
            usedWorkers += room.CurrentWorkers;
        }
        Debug.Log($"전체 일꾼: {totalWorkers}, 일 하는 중: {usedWorkers}, 대기 중: {availableWorkers}");
        /*
        // UI 업데이트 로직
        if (WorkerManagementUI.instance != null)
        {
            WorkerManagementUI.instance.UpdateUI();
        }
        */
    }
}
