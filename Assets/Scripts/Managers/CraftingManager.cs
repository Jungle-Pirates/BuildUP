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

        // 아이템 제작 로직
        StartCoroutine(CraftItemCoroutine(recipe));
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
    /// 제작 시작 시 호출될 함수
    /// </summary>
    private void OnCraftingStarted(Recipe recipe)
    {
        // TODO: 필요한 재료 소모 처리
        // foreach (var requiredItem in recipe.requiredItem)
        // {
        //     player.Inventory.RemoveItem(requiredItem.itemID, requiredItem.amount);
        // }
    }

    /// <summary>
    /// 제작 완료 처리
    /// </summary>
    private void CompleteCrafting(Recipe recipe)
    {
        // TODO: 결과 아이템 인벤토리에 추가
        // player.Inventory.AddItem(recipe.resultItemID, recipe.craftAmount);
    }
}
