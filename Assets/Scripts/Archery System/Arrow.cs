using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float maxLifeTime = 3f;  // 화살 최대 수명
    private float currentLifeTime = 0f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // 활성화될 때 타이머 초기화
        currentLifeTime = 0f;
    }

    void Update()
    {
        // 수명 체크
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= maxLifeTime)
        {
            ReturnToPool();
        }

        float fDrag = 0.5f * 1f * rb.velocity.magnitude * rb.velocity.magnitude * 0.001f * Time.deltaTime;
        rb.velocity -= rb.velocity.normalized * fDrag;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 시 풀로 돌아가기
        ReturnToPool();
    }

    void ReturnToPool()
    {
        // 발사한 ArcherController 찾아서 화살을 풀로 반환
        Bow archerController = FindObjectOfType<Bow>();
        if (archerController != null)
        {
            archerController.ReturnArrowToPool(gameObject);
        }
    }
}
