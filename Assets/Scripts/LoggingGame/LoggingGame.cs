using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoggingGame : MonoBehaviour
{
    [FormerlySerializedAs("Slider")] [FormerlySerializedAs("powerSlider")] [Header("UI Components")]
    public Slider slider;
    public Slider powerSlider;
    public Image hitPointMarker;
    public Image currentPointMarker;

    [Header("Game Settings")]
    public float sliderSpeed = 50f;
    public float hitPointSize = 10f;

    [Header("Game State")]
    private float currentSliderValue;
    private float hitPointPosition;
    private float damage;
    private bool isDescending = true;
    private bool isGameActive = true;

    void Start()
    {
        // 슬라이더 초기 설정
        slider.minValue = 0f;
        slider.maxValue = 100f;
        currentSliderValue = 0f;

        // 타격 포인트 랜덤 배치
        SetRandomHitPoint();
    }

    void Update()
    {
        if (!isGameActive)
        {
            HandleRightClickInput();
        }
        else
        {
            // 슬라이더 움직임 로직
            UpdateSliderMovement();

            // 좌클릭 입력 처리
            HandleLeftClickInput();
        }
    }
    
    void UpdateSliderMovement()
    {
        if (Input.GetMouseButton(0))
        {
            if (isDescending)
            {
                currentSliderValue -= sliderSpeed * Time.deltaTime;

                if (currentSliderValue <= 0f)
                {
                    isDescending = false;
                }
            }
            else
            {
                currentSliderValue += sliderSpeed * Time.deltaTime;

                if (currentSliderValue >= 100f)
                {
                    isDescending = true;
                }
            }

            slider.value = currentSliderValue;
            currentPointMarker.rectTransform.anchorMax = new Vector2(1f, currentSliderValue / 100f);
            powerSlider.value += Time.deltaTime;
        }
    }

    void HandleLeftClickInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Mathf.Abs(currentSliderValue - hitPointPosition) <= hitPointSize/2)
            {
                // 성공: 나무 베기 애니메이션, 점수 증가 등
                Debug.Log("나무 베기 성공!");
                ChopWood();
            }
            else
            {
                // 실패: 실패 처리
                Debug.Log("타격 실패!");
                GameOver();
            }
            powerSlider.value = 0;
        }
    }
    
    private void HandleRightClickInput()
    {
        if (Input.GetMouseButtonDown(1) && isGameActive == false)
        {
            isGameActive = true;
            SetRandomHitPoint();
        }
    }


    void SetRandomHitPoint()
    {
        hitPointPosition = Random.Range(5f, 95f);
        hitPointMarker.rectTransform.anchoredPosition = new Vector2(0, hitPointPosition * 5f - 250f);
    }

    void ChopWood()
    {
        damage += powerSlider.value;
        if (damage >= 3)
        {
            damage = 0;
            SetRandomHitPoint();
        }
    }

    void GameOver()
    {
        damage = 0;
        isGameActive = false;
    }
}