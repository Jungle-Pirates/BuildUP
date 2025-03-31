using UnityEngine;
using Mirror;
/// <summary>
/// 아이템 생성, 삭제를 관리하는 클래스
/// </summary>
public class NetworkItemManager : Singleton<NetworkItemManager>
{
    [SerializeField]
    private NetworkItem itemPrefab;

    /// <summary>
    /// 월드에 아이템을 생성하고, 모든 클라이언트에게도 생성합니다.
    /// </summary>
    /// <param name="itemCode">생성할 아이템 코드</param>
    /// <param name="position">생성할 위치</param>
    /// <param name="isDrop">랜덤으로 흩뿌려져야 하면 true, 지정된 장소에 떨어져야 하면 false </param>
    [Server]
    public void SpawnItem(string itemCode, Vector3 position, bool isDrop)
    {
        GameObject dropItem = Instantiate(itemPrefab, position, Quaternion.identity).gameObject;

        NetworkItem item = dropItem.GetComponent<NetworkItem>();
        if (item == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>(그럴일은 없겠지만)생성할 아이템에 newtworkItem 컴포넌트가 없습니다.");
            Destroy(dropItem);
            return;
        }
        // 아이템이 null이 아니면 아이템 정보를 설정합니다.
        item.SetItemInfo(itemCode);
        // 아이템을 클라이언트모두에게 생성
        NetworkServer.Spawn(dropItem);

        // 아이템을 생성한 후, 아이템의 위치를 설정합니다.
        ItemDropMovement dropMovement = dropItem.GetComponent<ItemDropMovement>();
        // 만약 아이템 드롭 효과가 없다면, 아이템 드롭 효과 컴포넌트를 추가합니다.
        if (dropMovement == null)
        {
            dropItem.AddComponent<ItemDropMovement>();
            dropMovement = dropItem.GetComponent<ItemDropMovement>();
        }
        // 아이템 드롭 효과를 설정합니다.
        // 해당 메소드는 랜덤으로 흩뿌려주는 기능이 섞여서 버릴때와 드롭될때 구분해줘야함
        dropMovement.InitializePosition(position, isDrop);
    }
    /// <summary>
    /// 아이템을 파괴합니다. 각 클라이언트들에게도 아이템을 파괴시켜줍니다.
    /// </summary>
    /// <param name="item">파괴할 아이템 오브젝트</param>
    [Server]
    public void DestroyItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("<color=red>[에러 발생]</color>파괴할 아이템이 null입니다.");
            return;
        }
        NetworkServer.Destroy(item);
    }
}