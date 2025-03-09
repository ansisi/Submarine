using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeliverableItem : MonoBehaviour
{
    public bool isGrabbed = false;

    // �÷��̾ �� ������Ʈ�� ����� �� ȣ��
    public virtual void OnGrabbed()
    {
        isGrabbed = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;  // Ʈ���� Ȱ��ȭ (�������� ������)
            }
        }
    }

    // �÷��̾ ���� �� ȣ��
    public virtual void Release()
    {
        isGrabbed = false;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;  // Ʈ���� ��Ȱ��ȭ (���� ��)
            }
        }
    }

    // ����Կ� ���޵Ǿ��� �� ������ ���� (�ڽ� Ŭ�������� ����)
    public abstract void OnDelivered(Submarine submarine);

    // ������Ʈ�� �ı��� �� ȣ��Ǿ� availableItems���� �ش� �������� �����ϴ� �޼���
    private void OnDestroy()
    {
        PlayerPickup playerPickup = FindObjectOfType<PlayerPickup>();
        if (playerPickup != null)
        {
            playerPickup.RemoveAvailableItems(this);
        }
    }
}
