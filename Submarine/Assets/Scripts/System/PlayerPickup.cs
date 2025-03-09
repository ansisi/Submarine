using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Vector3 headOffset = new Vector3(0, 1.5f, 0); // 머리 위로의 오프셋

    private List<DeliverableItem> availableItems = new List<DeliverableItem>();
    private DeliverableItem currentHeldItem;

    private void OnTriggerEnter(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
            Logger.Log("픽업 후보 추가: " + item.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && availableItems.Contains(item))
        {
            availableItems.Remove(item);
            Logger.Log("픽업 후보 제거: " + item.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryGrabItem();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ReleaseItem();
        }

        // 잡고 있는 동안 아이템을 플레이어 머리 위로 고정 (로컬 좌표 사용)
        if (currentHeldItem != null)
        {
            
            Vector3 headPosition = transform.TransformPoint(headOffset); // 로컬 좌표 → 월드 좌표 변환
            currentHeldItem.transform.position = headPosition;
            currentHeldItem.transform.rotation = transform.rotation; // 플레이어 회전과 동기화
        }
    }

    void TryGrabItem()
    {
        if (currentHeldItem == null && availableItems.Count > 0)
        {
            DeliverableItem closestItem = GetClosestItem();
            if (closestItem != null)
            {
                currentHeldItem = closestItem;
                currentHeldItem.OnGrabbed();

                Logger.Log("가장 가까운 아이템 잡음: " + closestItem.name);
                availableItems.Remove(closestItem);
            }
        }
    }

    void ReleaseItem()
    {
        if (currentHeldItem != null)
        {
            // 아이템 놓기
            currentHeldItem.Release();

            Logger.Log("아이템 놓음: " + currentHeldItem.name);
            currentHeldItem = null;
        }
    }

    // 가장 가까운 아이템 찾기
    private DeliverableItem GetClosestItem()
    {
        DeliverableItem closestItem = null;
        float minDistance = Mathf.Infinity;
        foreach (DeliverableItem item in availableItems)
        {
            
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestItem = item;
            }
        }
        return closestItem;
    }

    // availableItems에서 아이템을 제거하는 메서드
    public void RemoveAvailableItems(DeliverableItem item)
    {
        if (availableItems.Contains(item))
        {
            availableItems.Remove(item);
            Logger.Log("파괴된 아이템 제거: " + item.name);
        }
    }
}
