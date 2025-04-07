using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레시피가 열리며 다양한 제작들이 가능한 방 객체 (ex. 제련방, 방앗간, 취사장)
/// </summary>
public abstract class CraftingRoom : Room
{
    [Header("제작방 UI")]
    [SerializeField] private GameObject craftingUI; // 농장 UI
    [SerializeField] private GameObject craftingQueueUI; // 제작 큐 UI
    [Header("제작 큐")]
    public int maxQueueCount = 8; // 최대 큐 개수
    public Queue<Recipe> craftingQueue = new Queue<Recipe>(); // 제작 큐
    public Coroutine craftingCoroutine; // 제작 코루틴
    protected override void Start()
    {
        base.Start();
        craftingUI.transform.Find("UItitle").GetComponent<TextMeshProUGUI>().text = roomType.ToString(); // 방 이름 설정
        craftingUI.SetActive(false);
        UpdateQueueUI();
    }
    public override void OpenRoomUI()
    {
        if (IsOccupied) // 방이 사용중이면 UI를 열지 않음
        {
            return;
        }
        // 농장 UI 열기
        if (craftingUI != null)
        {
            craftingUI.SetActive(true);
        }
    }
    public override void CloseRoomUI()
    {
        // 농장 UI 닫기
        if (craftingUI != null)
        {
            craftingUI.SetActive(false);
        }
    }
    public void AddCraftingQueue(string ItemID)
    {
        if (IsOccupied) // 방이 사용중이면 큐에 추가하지 않음
        {
            Debug.LogWarning($"방이 사용중입니다: {ItemID}");
            return;
        }
        if (craftingQueue.Count >= maxQueueCount)
        {
            Debug.LogWarning($"제작 큐가 가득 찼습니다: {ItemID}");
            return;
        }
        Recipe recipe = CraftingManager.Instance.GetRecipeByItemID(ItemID);
        if (recipe == null)
        {
            Debug.LogError($"레시피를 찾을 수 없음: {ItemID}");
            return;
        }
        if (!CraftingManager.Instance.IsAbleToCraft(recipe))
        {
            Debug.LogWarning($"재료가 부족하여 제작할 수 없음: {ItemID}");
            return;
        }
        // 레시피가 있다면 큐에 추가
        craftingQueue.Enqueue(recipe);
        // 큐에 추가하면서 재료아이템은 소모
        CraftingManager.Instance.OnCraftingStarted(recipe);
        // 큐 UI 업데이트
        UpdateQueueUI();
        if (craftingCoroutine == null)
        {
            SetOccupied(true); // 제작 시작 시 Occupy 설정
            craftingCoroutine = StartCoroutine(CraftingCoroutine());
        }
    }

    /// <summary>
    /// 아이템 제작 코루틴 (제작 시간 지연 처리)
    /// </summary>
    IEnumerator CraftingCoroutine()
    {
        while (craftingQueue.Count > 0)
        {
            Recipe recipe = craftingQueue.Peek();

            // 제작 시간만큼 대기
            float craftingTime = recipe.craftTime;
            float elapsedTime = 0f;
            Image progressBar = craftingQueueUI.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            progressBar.fillAmount = 0f; // 진행 바 초기화
            while (elapsedTime < craftingTime)
            {
                elapsedTime += Time.deltaTime;
                // UI 업데이트 (예: 진행 바, 텍스트 등)
                progressBar.fillAmount = elapsedTime / craftingTime;
                yield return null; // 다음 프레임까지 대기
            }

            // 제작 완료 처리
            craftingQueue.Dequeue(); // 큐에서 레시피 제거
            CraftingManager.Instance.OnCompleteCrafting(recipe);
            UpdateQueueUI();
        }
        craftingCoroutine = null;
        // 제작 종료하면 Occupy 해제
        SetOccupied(false);
    }
    private void UpdateQueueUI()
    {
        // 큐 UI 업데이트 로직 (예: 큐에 있는 레시피 목록 표시 등)
        if (craftingQueueUI != null)
        {
            craftingQueueUI.SetActive(craftingQueue.Count > 0);
            for (int i = 0; i < craftingQueueUI.transform.childCount; i++)
            {
                craftingQueueUI.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < craftingQueue.Count; i++)
            {
                craftingQueueUI.transform.GetChild(i).gameObject.SetActive(true);
                craftingQueueUI.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = craftingQueue.ToArray()[i].recipeName;
            }
        }
    }
}
