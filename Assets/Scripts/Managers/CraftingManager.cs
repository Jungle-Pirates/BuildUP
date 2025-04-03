using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : Singleton<CraftingManager>
{
    [Header("제작 레시피 목록")]
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();

    /// <summary>
    /// 레시피 ID로 레시피 찾기
    /// </summary>
    public Recipe GetRecipeByName(string recipeName)
    {
        return recipes.Find(recipe => recipe.recipeName == recipeName);
    }
    
    /// <summary>
    /// 레시피 아이템 ID로 레시피 찾기
    /// </summary>
    public Recipe GetRecipeByItemID(string itemID)
    {
        return recipes.Find(recipe => recipe.resultItemID == itemID);
    }

    /// <summary>
    /// 모든 레시피 가져오기
    /// </summary>
    public List<Recipe> GetAllRecipes()
    {
        return recipes;
    }

    /// <summary>
    /// 아이템 제작 함수
    /// </summary>
    /// <param name="itemID">제작할 아이템 아이디</param>
    public void CraftItem(string itemID)
    {
        Recipe recipe = GetRecipeByItemID(itemID);
        if (recipe == null)
        {
            Debug.LogError($"레시피를 찾을 수 없음: {itemID}");
            return;
        }

        //인벤토리에서 재료 아이템이 전부 존재하는지 체크 >> 제작 가능 여부 반환(bool)
        if(IsAbleToCraft(recipe))
        {
            // 아이템 제작 로직
            StartCoroutine(CraftItemCoroutine(recipe));
        }
        else
        {
            //TODO : 제작 불가능 안내 UI 띄우기
            Debug.LogWarning($"재료가 부족하여 제작할 수 없음: {itemID}");
            
        }
    }

    /// <summary>
    /// 아이템 제작 코루틴 (제작 시간 지연 처리)
    /// </summary>
    private IEnumerator CraftItemCoroutine(Recipe recipe)
    {
        // 제작 시작 알림 (UI 업데이트 등)
        OnCraftingStarted(recipe);

        // 제작 시간만큼 대기
        yield return new WaitForSeconds(recipe.craftTime);

        // 제작 완료 처리
        CompleteCrafting(recipe);
    }

    /// <summary>
    /// 플레이어의 인벤토리를 체크해서 제작 가능 여부를 반환
    /// </summary>
    public bool IsAbleToCraft(Recipe recipe)
    {
        foreach (var required in recipe.requiredItem)
        {
            if (!InventoryManager.Instance.HasItemAmount(required.itemID, required.amount))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 제작 시작 시 호출될 함수
    /// 레시피 재료 요구량만큼 재료 삭제
    /// </summary>
    private void OnCraftingStarted(Recipe recipe)
    {
        foreach (var required in recipe.requiredItem)
        {
            InventoryManager.Instance.RemoveItem(required.itemID, required.amount);
        }

        // UI 업데이트 등 추가 작업 필요 시 여기에
        Debug.Log($"[{recipe.recipeName}] 제작에 필요한 재료를 소모했습니다.");

        //자원 보유 현황에 변동이 생기므로 UI 갱신 한번 해주기
        bool canCraft = IsAbleToCraft(recipe);
        InventoryManager.Instance.craftButton.gameObject.SetActive(canCraft);
        InventoryManager.Instance.craftWarningText.text = canCraft ? "" : "재료가 부족합니다.";
        InventoryManager.Instance.craftWarningText.gameObject.SetActive(!canCraft);
    }

    /// <summary>
    /// 제작 완료 처리
    /// </summary>
    private void CompleteCrafting(Recipe recipe)
    {
        InventoryManager.Instance.AddItem(recipe.resultItemID, recipe.craftAmount);
        Debug.Log($"[{recipe.recipeName}] 제작 완료! {recipe.resultItemID} ×{recipe.craftAmount} 추가됨.");
    }
}
