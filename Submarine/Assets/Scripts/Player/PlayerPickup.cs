using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Vector3 headOffset = new Vector3(0, 1.5f, 0); // 머리 위로의 오프셋

    private List<DeliverableItem> availableItems = new List<DeliverableItem>();
    private DeliverableItem currentHeldItem;

    public HookController hookController;
    public HarpoonController harpoonController;

    public bool IsGrabbing => currentHeldItem != null;

    private void OnTriggerEnter(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
            //item.ShowPickupUI();
            Logger.Log("픽업 후보 추가: " + item.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && availableItems.Contains(item))
        {
            availableItems.Remove(item);
            //item.HidePickupUI();
            Logger.Log("픽업 후보 제거: " + item.name);
        }
    }

    void Update()
    {
        if (hookController.isHookActive == false && harpoonController.isHarpoonActive == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryGrabItem();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                ReleaseItem();
            }
        }

        // 잡고 있는 동안 아이템을 플레이어 머리 위로 고정 (로컬 좌표 사용)
        if (currentHeldItem != null)
        {
            
            Vector3 headPosition = transform.TransformPoint(headOffset); // 로컬 좌표 → 월드 좌표 변환
            currentHeldItem.transform.position = headPosition;

            Quaternion rotationOffset = Quaternion.Euler(0, 0, 90);     // 잡힌 아이템 z축 90도 회전
            currentHeldItem.transform.rotation = transform.rotation * rotationOffset; // 플레이어 회전과 동기화
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

                // RotationObject 스크립트 비활성화
                RotationObject rotationObject = currentHeldItem.GetComponent<RotationObject>();
                if (rotationObject != null)
                {
                    rotationObject.enabled = false; // 잡을 때 rotationObject 비활성화
                }

                Quaternion rotationOffset = Quaternion.Euler(0, 0, 90);
                currentHeldItem.transform.rotation = transform.rotation * rotationOffset;

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

            // Coroutine으로 1초 후에 RotationObject 활성화
            StartCoroutine(EnableRotationObjectAfterDelay(currentHeldItem, 1f)); // 1초 후에 활성화
            Logger.Log("아이템 놓음: " + currentHeldItem.name);
            currentHeldItem = null;
        }
    }

    // 1초 후에 RotationObject를 활성화하는 코루틴
    private IEnumerator EnableRotationObjectAfterDelay(DeliverableItem item, float delay)
    {
        yield return new WaitForSeconds(delay);

        RotationObject rotationObject = item.GetComponent<RotationObject>();
        if (rotationObject != null)
        {
            rotationObject.enabled = true; // 일정 시간 후에 rotationObject 활성화
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
