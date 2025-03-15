using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeliverableItem : MonoBehaviour
{
    public bool isGrabbed = false;

    // 플레이어가 이 오브젝트를 잡았을 때 호출
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
                collider.isTrigger = true;  // 트리거 활성화 (아이템이 잡히면)
            }
        }
    }

    // 플레이어가 놓을 때 호출
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
                collider.isTrigger = false;  // 트리거 비활성화 (놓을 때)
            }
        }
    }

    // 잠수함에 전달되었을 때 실행할 로직 (자식 클래스에서 구현)
    public abstract void OnDelivered(Submarine submarine);

    // 오브젝트가 파괴될 때 호출되어 availableItems에서 해당 아이템을 제거하는 메서드
    private void OnDestroy()
    {
        PlayerPickup playerPickup = FindObjectOfType<PlayerPickup>();
        if (playerPickup != null)
        {
            playerPickup.RemoveAvailableItems(this);
        }
    }
}
