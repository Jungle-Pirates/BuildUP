using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [Header("Room Upgrade Content")]
    [SerializeField] private List<RoomUpgradeUIContent> upgradeRooms = new List<RoomUpgradeUIContent>();
    [SerializeField] private Transform upgradeRoomsParent;
    [SerializeField] private GameObject roomUpgradeContentPrefab;

    private void Start()
    {
        UpdateUI();
        gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        upgradeRooms.Clear();

        // 방 리스트 피벗의 자식을 모두 삭제
        foreach (Transform child in upgradeRoomsParent)
        {
            Destroy(child.gameObject);
        }

        // 레시피 출력 피벗에 레시피 적용
        for (int i = 0; i < BuildUpgradeManager.Instance.Rooms.Length; i++)
        {
            RoomUpgradeUIContent resource = Instantiate(roomUpgradeContentPrefab, upgradeRoomsParent).GetComponent<RoomUpgradeUIContent>();
            resource.SetRoomData(BuildUpgradeManager.Instance.Rooms[i].GetComponent<Room>(), i);
            upgradeRooms.Add(resource);
        }
    }
}
