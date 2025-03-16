using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class DeliverableItem : MonoBehaviour
{
    public bool isGrabbed = false;

    private GameObject pickupUI;
    private RectTransform uiRectTransform;
    private Canvas mainCanvas;

    private void Start()
    {
        if (pickupUI == null)
        {
            GameObject uiPrefab = Resources.Load<GameObject>("PickupUI"); // "Resources" 폴더에서 UI 프리팹 로드
            mainCanvas = FindObjectOfType<Canvas>(); // 씬에 있는 Canvas 찾기

            if (uiPrefab != null && mainCanvas != null)
            {
                pickupUI = Instantiate(uiPrefab, mainCanvas.transform, false); // Canvas의 자식으로 생성
                uiRectTransform = pickupUI.GetComponent<RectTransform>();
                pickupUI.SetActive(false);
            }
        }
    }

    // 플레이어가 이 오브젝트를 잡았을 때 호출
    public virtual void OnGrabbed()
    {
        isGrabbed = true;
        pickupUI?.SetActive(false);
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

    public void ShowPickupUI()
    {
        if (!isGrabbed && pickupUI != null)
        {
            pickupUI.SetActive(true);
            UpdatePickupUIPosition();
        }
    }

    public void HidePickupUI()
    {
        if (pickupUI != null)
        {
            pickupUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (pickupUI != null && pickupUI.activeSelf)
        {
            UpdatePickupUIPosition();
        }
    }

    private void UpdatePickupUIPosition()
    {
        if (pickupUI != null && mainCanvas != null)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            uiRectTransform.position = screenPosition; // UI 좌표 설정
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
