using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Bow Settings")]
    public GameObject arrowPrefab;  // 화살 프리팹
    public float bowAngle;  // 활 위치
    public float arrowSpeed = 100f;  // 화살 속도
    public float maxDrawTime = 1.5f;  // 최대 활 당기기 시간
    public int poolSize = 20;  // 오브젝트 풀 크기

    private float _currentDrawTime = 0f;
    private bool _isDrawing = false;

    // 화살 오브젝트 풀
    private List<GameObject> arrowPool;

    void Start()
    {
        // 오브젝트 풀 초기화
        InitializeArrowPool();
    }

    void InitializeArrowPool()
    {
        arrowPool = new List<GameObject>();
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }
    }

    GameObject GetPooledArrow()
    {
        // 비활성화된 화살 찾아 반환
        foreach (GameObject arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
            {
                return arrow;
            }
        }

        // 모든 화살이 사용 중이면 새로 생성 (선택적)
        GameObject newArrow = Instantiate(arrowPrefab);
        arrowPool.Add(newArrow);
        return newArrow;
    }

    void Update()
    {
        // 사이드뷰 조준 로직
        HandleAiming();

        // 활 쏘기 로직
        HandleShooting();
    }

    void HandleAiming()
    {
        // 마우스 위치에 따라 활 각도 조정
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (mousePosition - transform.position).normalized;

        bowAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    void HandleShooting()
    {
        // 좌클릭으로 활 당기기
        if (Input.GetMouseButtonDown(0))
        {
            _isDrawing = true;
            _currentDrawTime = 0f;
        }

        // 활 당기는 시간 계산
        if (_isDrawing)
        {
            _currentDrawTime += Time.deltaTime;
            _currentDrawTime = Mathf.Clamp(_currentDrawTime, 0f, maxDrawTime);
        }

        // 마우스 버튼 놓으면 화살 발사
        if (Input.GetMouseButtonUp(0) && _isDrawing)
        {
            ShootArrow();
            _isDrawing = false;
            _currentDrawTime = 0f;
        }
    }

    void ShootArrow()
    {
        // 오브젝트 풀에서 화살 가져오기
        GameObject arrow = GetPooledArrow();
        
        // 화살 위치, 회전 설정
        arrow.transform.position = transform.position;
        arrow.transform.rotation = Quaternion.Euler(0f,0f,bowAngle);
        arrow.SetActive(true);

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        
        // 활 당긴 시간에 따라 화살 속도 조절
        float normalizedDrawTime = _currentDrawTime / maxDrawTime;
        float finalArrowSpeed = arrowSpeed * normalizedDrawTime;
        
        arrowRb.velocity = arrow.transform.right * finalArrowSpeed;
    }

    // 화살 비활성화 메서드 (다른 스크립트에서 호출)
    public void ReturnArrowToPool(GameObject arrow)
    {
        arrow.SetActive(false);
    }
}
