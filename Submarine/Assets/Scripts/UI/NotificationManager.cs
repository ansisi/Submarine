using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public ItemNotificationUI notificationUI; // 씬에 배치한 프리팹 인스턴스 참조

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>ItemPickup에서 호출</summary>
    public void ShowPickup(Item item, int quantity)
    {
        notificationUI.Show(item, quantity);
    }
}
