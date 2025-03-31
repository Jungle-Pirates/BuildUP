using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("Rock Settings")]
    public float maxShakeAmount = 0.1f;
    public int requiredHitPoints = 5;
    public float hitPointRadius = 0.5f;
    public float rockRadius = 2.5f;
    
    [Header("Game Objects")]
    public GameObject hitPointPrefab;
    
    // Internal variables
    private Vector3 originalPosition;
    private List<GameObject> hitPoints = new List<GameObject>();
    private List<bool> hitPointsStatus = new List<bool>();
    private bool isMouseDown = false;
    private float mouseDownTime = 0f;
    private float minHoldTime = 0.5f;
    
    void Start()
    {
        originalPosition = transform.position;
        GenerateNewRock();
    }
    
    void Update()
    {
        HandleInput();
        CheckRockStatus();
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
            mouseDownTime = 0f;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            releasePosition.z = 0;
            
            if (isMouseDown && mouseDownTime >= minHoldTime)
            {
                CheckHitPointSuccess(releasePosition);
                float closestDistance = CalculateClosestHitPointDistance(releasePosition);
                ShakeRock(closestDistance);
            }
            if (isMouseDown && mouseDownTime < minHoldTime)
            {
                float closestDistance = CalculateClosestHitPointDistance(releasePosition);
                ShakeRock(closestDistance);
            }
            
            isMouseDown = false;
            mouseDownTime = 0f;
        }
        
        if (isMouseDown)
        {
            mouseDownTime += Time.deltaTime;
        }
    }
    
    float CalculateClosestHitPointDistance(Vector3 position)
    {
        float closestDistance = float.MaxValue;
        
        foreach (GameObject hitPoint in hitPoints)
        {
            float distance = Vector3.Distance(hitPoint.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        
        return closestDistance;
    }
    
    void ShakeRock(float distance)
    {
        // 타격 포인트에 가까울수록 흔들림이 커짐
        float normalizedDistance = Mathf.Clamp01(1 - (distance / (hitPointRadius * 2)));
        float shakeAmount = normalizedDistance * maxShakeAmount;
        
        StopAllCoroutines();
        StartCoroutine(ShakeEffect(shakeAmount));
    }
    
    IEnumerator ShakeEffect(float amount)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * amount;
            randomOffset.z = 0;
            transform.position = originalPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = originalPosition;
    }
    
    void CheckHitPointSuccess(Vector3 position)
    {
        for (int i = 0; i < hitPoints.Count; i++)
        {
            if (!hitPointsStatus[i])
            {
                float distance = Vector3.Distance(hitPoints[i].transform.position, position);
                if (distance <= hitPointRadius)
                {
                    // 타격 성공
                    hitPointsStatus[i] = true;
                    hitPoints[i].GetComponent<SpriteRenderer>().color = Color.gray;
                }
            }
        }
    }
    
    void CheckRockStatus()
    {
        bool allHit = true;
        
        foreach (bool status in hitPointsStatus)
        {
            if (!status)
            {
                allHit = false;
                break;
            }
        }
        
        if (allHit && hitPointsStatus.Count > 0)
        {
            // 모든 타격 포인트 처리 완료
            StartCoroutine(DestroyRockAndCreateNew());
        }
    }
    
    IEnumerator DestroyRockAndCreateNew()
    {
        // 바위 파괴 애니메이션 효과
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsed / duration);
            transform.localScale = new Vector3(scale, scale, 1);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 새 바위 생성
        GenerateNewRock();
    }
    
    void GenerateNewRock()
    {
        // 기존 타격 포인트 제거
        foreach (GameObject hitPoint in hitPoints)
        {
            Destroy(hitPoint);
        }
        
        hitPoints.Clear();
        hitPointsStatus.Clear();
        
        // 바위 초기화
        transform.position = originalPosition;
        transform.localScale = 2 * rockRadius * Vector3.one;
        
        // 랜덤한 타격 포인트 생성
        int numHitPoints = requiredHitPoints; // Random.Range(2, requiredHitPoints + 1);
        
        for (int i = 0; i < numHitPoints; i++)
        {
            // 바위 주변에 랜덤한 위치 생성
            float angle = Random.Range(0f, 360f); // (360f / numHitPoints) * i;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * Random.Range(0.2f, 0.8f) * rockRadius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * Random.Range(0.2f, 0.8f) * rockRadius,
                0
            );
            
            GameObject newHitPoint = Instantiate(hitPointPrefab, transform.position + offset, Quaternion.identity);
            newHitPoint.transform.SetParent(transform);
            hitPoints.Add(newHitPoint);
            hitPointsStatus.Add(false);
        }
    }
}