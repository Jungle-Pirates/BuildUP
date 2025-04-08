using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUpgradeUIContent : MonoBehaviour
{
    [Header("Resource UI")]
    [SerializeField] private List<RoomUpgradeUIResourceContent> resourceContents = new List<RoomUpgradeUIResourceContent>();
    [SerializeField] private Transform resourceContentsParent;
    [SerializeField] private GameObject resourceContentPrefab;

    [Header("Room UI")]
    [SerializeField] private Image roomImage;
    [SerializeField] private TextMeshProUGUI roomNameText;

    private Room _data;
    private int _prefabIndex;

    /// <summary>
    /// 해당하는 방의 방 데이터를 설정하고 UI를 업데이트하는 메서드
    /// </summary>
    /// <param name="data"></param>
    public void SetRoomData(Room data, int index)
    {
        this._data = data;
        _prefabIndex = index;
        UpdateUI();
    }

    /// <summary>
    /// 가지고 있는 방의 데이터를 통해서 UI를 업데이트 하는 함수
    /// </summary>
    private void UpdateUI()
    {
        // 레시피 출력 피벗의 자식을 모두 삭제
        foreach(Transform child in resourceContentsParent)
        {
            Destroy(child.gameObject);
        }

        // 레시피 출력 피벗에 레시피 적용
        for (int i = 0; i < _data.RequiredItems.Length; i++)
        {
            RoomUpgradeUIResourceContent resource = Instantiate(resourceContentPrefab, resourceContentsParent).GetComponent<RoomUpgradeUIResourceContent>();
            resource.SetData(_data.RequiredItems[i].itemID, _data.RequiredItems[i].amount);
            resourceContents.Add(resource);
        }

        // 방의 이미지와 이름 적용
        roomImage.sprite = _data.RoomSprite;
        roomNameText.text = _data.RoomName;
    }

    public void SetSelectedRoom()
    {
        BuildManager.Instance.SetSelectedRoom(_data, _prefabIndex);
    }
}
