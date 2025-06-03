using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : InteractableBase
{
    public Item item;
    [Min(1)]
    public int quantity = 1;

    private Coroutine destroyTimerCoroutine;
    private bool isVisible = true; // 처음엔 보이는 상태로 간주

    private void Start()
    {
        InteractionManager.instance.Register(this);
    }

    private void OnDestroy()
    {
        InteractionManager.instance.Unregister(this);
    }

    public override string GetHintText()
    {
        return $"[Space] {item.itemName} 줍기";
    }

    public override void Interact()
    {
        bool success = InventoryManager.Instance.AddItem(item, quantity);
        if (success)
        {
            if (destroyTimerCoroutine != null)
            {
                StopCoroutine(destroyTimerCoroutine);
                destroyTimerCoroutine = null;
            }

            QuestEventSystem.Raise(QuestActionType.CollectResource, item.itemName);

            // “줍기”한 경우에만 알림 호출
            NotificationManager.Instance.ShowPickup(item, quantity);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 시야 범위 트리거에서 호출: true일 경우 파괴 타이머 중단, false일 경우 타이머 시작
    /// </summary>
    public void SetVisible(bool visible)
    {
        isVisible = visible;

        if (visible)
        {
            // 다시 보이게 되면 타이머 정지
            if (destroyTimerCoroutine != null)
            {
                StopCoroutine(destroyTimerCoroutine);
                destroyTimerCoroutine = null;
            }
        }
        else
        {
            // 처음 사라질 때만 타이머 시작
            if (destroyTimerCoroutine == null)
            {
                destroyTimerCoroutine = StartCoroutine(DestroyAfterDelay(10f));
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        float timer = 0f;

        while (timer < delay)
        {
            // 중간에 다시 시야에 들어오면 타이머 취소
            if (isVisible)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public override int Priority => 0; // 우선순위 높음
}
