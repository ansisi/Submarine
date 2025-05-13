using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideDrone : Enemy
{
    // 자폭 시 이펙트 프리팹 (선택사항)
    public GameObject explosionEffect;

    protected override void Attack()
    {
        // 자폭 조건: 타겟이 존재하고 일정 거리 이내
        if (target != null)
        {
            float dist = GetDistanceToTargetEdge();
            if (dist <= attackRange)
            {
                // 타겟이 피해를 받을 수 있다면 데미지 주기
                IDamageable dmg = target.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }

                // 폭발 이펙트 생성 (선택)
                if (explosionEffect != null)
                {
                    Instantiate(explosionEffect, transform.position, Quaternion.identity);
                }

                // 자폭 후 본인 제거
                Die();
            }
        }
    }

    // 추가: 자폭은 한 번만 하므로 공격 코루틴은 사용 안 함
    protected void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
}
