using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Notification : Singleton<Notification>
{
    [SerializeField]
    private GameObject notificationPrefab; // 알림 텍스트 프리팹
    [SerializeField]
    private Transform notificationParent; // 알림 텍스트 부모
    [SerializeField]
    [Header("알림 설정")]
    [Tooltip("알림 표시 시간")]
    private float notificationDuration = 3.0f;
    [Tooltip("최대 알림 개수")]
    [SerializeField]
    private int maxNotifications = 5;
    private List<GameObject> activeNotifications = new List<GameObject>(); // 현재 활성화된 알림 목록

    /// <summary>
    /// 알림 메시지 생성
    /// </summary>
    /// <param name="message">알림 메시지</param>
    public void CreateNotification(string message)
    {
        // 최대 알림 개수 확인 및 관리
        if (activeNotifications.Count >= maxNotifications)
        {
            // 가장 오래된 알림(리스트의 첫 번째 항목) 제거
            RemoveOldestNotification();
        }
        // 알림 프리팹을 복제하여 알림 메시지 생성
        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        var textComponent = notification.GetComponent<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = message;
        }

        // 활성 알림 목록에 추가
        activeNotifications.Add(notification);

        // 일정 시간 후 알림 제거
        StartCoroutine(RemoveNotificationAfterDelay(notification, notificationDuration));
    }

    /// <summary>
    /// 일정 시간 후 알림 제거
    /// </summary>
    private IEnumerator RemoveNotificationAfterDelay(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 알림이 아직 활성 상태인지 확인
        if (notification != null && activeNotifications.Contains(notification))
        {
            activeNotifications.Remove(notification);
            Destroy(notification);
        }
    }

    /// <summary>
    /// 가장 오래된 알림 제거
    /// </summary>
    private void RemoveOldestNotification()
    {
        if (activeNotifications.Count > 0)
        {
            GameObject oldestNotification = activeNotifications[0];
            activeNotifications.RemoveAt(0);
            Destroy(oldestNotification);
        }
    }
}