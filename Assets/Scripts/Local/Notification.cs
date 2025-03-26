using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Notification : Singleton<Notification>
{
    [SerializeField]
    private GameObject notificationPrefab; // �˸� �ؽ�Ʈ ������
    [SerializeField]
    private Transform notificationParent; // �˸� �ؽ�Ʈ �θ�
    [SerializeField]
    [Header("�˸� ����")]
    [Tooltip("�˸� ǥ�� �ð�")]
    private float notificationDuration = 3.0f;
    [Tooltip("�ִ� �˸� ����")]
    [SerializeField]
    private int maxNotifications = 5;
    private List<GameObject> activeNotifications = new List<GameObject>(); // ���� Ȱ��ȭ�� �˸� ���

    /// <summary>
    /// �˸� �޽��� ����
    /// </summary>
    /// <param name="message">�˸� �޽���</param>
    public void CreateNotification(string message)
    {
        // �ִ� �˸� ���� Ȯ�� �� ����
        if (activeNotifications.Count >= maxNotifications)
        {
            // ���� ������ �˸�(����Ʈ�� ù ��° �׸�) ����
            RemoveOldestNotification();
        }
        // �˸� �������� �����Ͽ� �˸� �޽��� ����
        GameObject notification = Instantiate(notificationPrefab, notificationParent);
        var textComponent = notification.GetComponent<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = message;
        }

        // Ȱ�� �˸� ��Ͽ� �߰�
        activeNotifications.Add(notification);

        // ���� �ð� �� �˸� ����
        StartCoroutine(RemoveNotificationAfterDelay(notification, notificationDuration));
    }

    /// <summary>
    /// ���� �ð� �� �˸� ����
    /// </summary>
    private IEnumerator RemoveNotificationAfterDelay(GameObject notification, float delay)
    {
        yield return new WaitForSeconds(delay);

        // �˸��� ���� Ȱ�� �������� Ȯ��
        if (notification != null && activeNotifications.Contains(notification))
        {
            activeNotifications.Remove(notification);
            Destroy(notification);
        }
    }

    /// <summary>
    /// ���� ������ �˸� ����
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