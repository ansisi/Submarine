using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    public float interactRange = 2f;

    // 우선순위 (낮을수록 우선)
    public virtual int Priority => 0;

    // 상호작용 텍스트
    public abstract string GetHintText();

    // 상호작용 키 처리
    public abstract void Interact();

    // 현재 플레이어와의 거리 계산
    public float DistanceToPlayer(Transform player)
    {
        return Vector3.Distance(transform.position, player.position);
    }

    // 플레이어가 근처에 있는지 확인
    public bool IsInRange(Transform player)
    {
        return DistanceToPlayer(player) <= interactRange;
    }
}
