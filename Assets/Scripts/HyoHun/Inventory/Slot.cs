using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �κ��丮 �� ���� ������ ����ϴ� Ŭ����
/// - ���Կ� ǥ���� ������ ����
/// - Ŭ�� �� ������ ���� ó��
/// - ������/���� UI ǥ�� ó�� ����
/// </summary>
public class Slot : MonoBehaviour
{
    public InventoryItem inventoryItem;         // �� ���Կ� ����ִ� �κ��丮 ������ ������

    public UIInventory inventory;               // �Ҽӵ� UIInventory ����
    public Button button;                       // ���� Ŭ���� ���� ��ư (�ɼ�)
    public Image icon;                          // ������ �̹��� ������
    public TextMeshProUGUI quantityText;        // ������ ���� �ؽ�Ʈ
    private Outline outline;                    // ���� �� ���� ȿ�� (�ð��� �׵θ�)

    public int index;                           // ���� ��ȣ (UIInventory���� �νĿ�)
    public int quantity;                        // ���� ���� (����� inventoryItem.count�� �ߺ�)

    private void Awake()
    {
        outline = GetComponent<Outline>();      // Outline ������Ʈ �ʱ�ȭ
    }

    private void OnEnable()
    {
        //���� �ߴ� �ڷῡ ��� ���� ���� �ڵ尡 �־���
    }

    /// <summary>
    /// ���Կ� ������ UI ������ ǥ���ϴ� �Լ�
    /// </summary>
    public void Set()
    {
        icon.gameObject.SetActive(true);                    // ������ ǥ��
        icon.sprite = inventoryItem.icon;                   // ������ �̹��� ����

        // ������ 2�� �̻��� ��쿡�� ���� ǥ��
        quantityText.text = inventoryItem.count > 1
            ? inventoryItem.count.ToString()
            : string.Empty;
    }

    /// <summary>
    /// ���� ���� - UI ��Ȱ��ȭ �� ������ ����
    /// </summary>
    public void Clear()
    {
        inventoryItem = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    /// <summary>
    /// ���� ��ư�� ������ �� ȣ���
    /// UIInventory�� ���� ���õ� �������� �˸�
    /// </summary>
    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}