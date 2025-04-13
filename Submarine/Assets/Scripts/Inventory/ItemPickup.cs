using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;               // 아이템 정보
    [Min(1)]
    public int quantity = 1;        // 줍는 수량
    public float pickupRange = 2f;  // 줍기 가능한 거리 반경

    private Transform player;

    private void Start()
    {
        // 플레이어 태그로 Transform 찾기
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Player 오브젝트에 'Player' 태그가 필요합니다.");
    }

    private void Update()
    {
        if (player == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupRange)
        {
            // E 키로 아이템 줍기
            PickupUIManager.instance.ShowHint(true, item.itemName);

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickup();
            }
        }
        else
        {
            PickupUIManager.instance.ShowHint(false);
        }
    }

    private void TryPickup()
    {
        // 스택 불가일 경우 quantity 무시
        int pickupAmount = item.isStackable ? quantity : 1;

        bool success = InventoryManager.instance.AddItem(item, quantity);
        if (success)
        {
            PickupUIManager.instance.ShowHint(false);
            Destroy(gameObject);
        }
    }

    // Scene 뷰에서 거리 시각화 (디버그용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // 선 색상
        Gizmos.DrawWireSphere(transform.position, pickupRange); // 원형 범위 표시
    }
}
