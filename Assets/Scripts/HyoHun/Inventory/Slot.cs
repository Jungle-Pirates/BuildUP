using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 내 개별 슬롯을 담당하는 클래스
/// - 슬롯에 표시할 아이템 정보
/// - 클릭 시 아이템 선택 처리
/// - 아이콘/수량 UI 표시 처리 포함
/// </summary>
public class Slot : MonoBehaviour
{
    public InventoryItem inventoryItem;         // 이 슬롯에 들어있는 인벤토리 아이템 데이터

    public UIInventory inventory;               // 소속된 UIInventory 참조
    public Button button;                       // 슬롯 클릭을 위한 버튼 (옵션)
    public Image icon;                          // 아이템 이미지 아이콘
    public TextMeshProUGUI quantityText;        // 아이템 수량 텍스트
    private Outline outline;                    // 선택 시 강조 효과 (시각적 테두리)

    public int index;                           // 슬롯 번호 (UIInventory에서 인식용)
    public int quantity;                        // 현재 수량 (현재는 inventoryItem.count와 중복)

    private void Awake()
    {
        outline = GetComponent<Outline>();      // Outline 컴포넌트 초기화
    }

    private void OnEnable()
    {
        //참고 했던 자료에 장비 장착 관련 코드가 있었음
    }

    /// <summary>
    /// 슬롯에 아이템 UI 정보를 표시하는 함수
    /// </summary>
    public void Set()
    {
        icon.gameObject.SetActive(true);                    // 아이콘 표시
        icon.sprite = inventoryItem.icon;                   // 아이템 이미지 적용

        // 수량이 2개 이상일 경우에만 수량 표시
        quantityText.text = inventoryItem.count > 1
            ? inventoryItem.count.ToString()
            : string.Empty;
    }

    /// <summary>
    /// 슬롯 비우기 - UI 비활성화 및 데이터 제거
    /// </summary>
    public void Clear()
    {
        inventoryItem = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    /// <summary>
    /// 슬롯 버튼이 눌렸을 때 호출됨
    /// UIInventory에 현재 선택된 슬롯으로 알림
    /// </summary>
    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}