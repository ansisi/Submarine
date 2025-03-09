using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Vector3 headOffset = new Vector3(0, 1.5f, 0); // �Ӹ� ������ ������

    private List<DeliverableItem> availableItems = new List<DeliverableItem>();
    private DeliverableItem currentHeldItem;

    private void OnTriggerEnter(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
            Logger.Log("�Ⱦ� �ĺ� �߰�: " + item.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && availableItems.Contains(item))
        {
            availableItems.Remove(item);
            Logger.Log("�Ⱦ� �ĺ� ����: " + item.name);
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

        // ��� �ִ� ���� �������� �÷��̾� �Ӹ� ���� ���� (���� ��ǥ ���)
        if (currentHeldItem != null)
        {
            
            Vector3 headPosition = transform.TransformPoint(headOffset); // ���� ��ǥ �� ���� ��ǥ ��ȯ
            currentHeldItem.transform.position = headPosition;
            currentHeldItem.transform.rotation = transform.rotation; // �÷��̾� ȸ���� ����ȭ
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

                Logger.Log("���� ����� ������ ����: " + closestItem.name);
                availableItems.Remove(closestItem);
            }
        }
    }

    void ReleaseItem()
    {
        if (currentHeldItem != null)
        {
            // ������ ����
            currentHeldItem.Release();

            Logger.Log("������ ����: " + currentHeldItem.name);
            currentHeldItem = null;
        }
    }

    // ���� ����� ������ ã��
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

    // availableItems���� �������� �����ϴ� �޼���
    public void RemoveAvailableItems(DeliverableItem item)
    {
        if (availableItems.Contains(item))
        {
            availableItems.Remove(item);
            Logger.Log("�ı��� ������ ����: " + item.name);
        }
    }
}
