using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    ClosestTurret,
    BuffTurret,
    Spaceship
}

[RequireComponent(typeof(FactionHandler))]
public class Enemy : MonoBehaviour, IDamageable
{
    // 목표 종류
    public TargetType targetType;            // ClosestTurret, BuffTurret, Spaceship 중 하나

    // 공통 속성
    public float speed = 3f;                 // 이동 속도
    public float attackRange = 1f;           // 사정거리 (이 거리 이내에서 Attack 호출)
    public float damage = 10f;               // 충돌 시 주는 대미지
    public float maxHealth = 50f;            // 최대 체력
    public float attackSpeed = 1f;           // 공격 속도

    protected float currentHealth;           // 현재 체력
    protected Transform target;              // 현재 타겟
    protected Coroutine damageCoroutine;
    protected LookAtTargetHandler lookAtHandler;

    void Start()
    {
        lookAtHandler = GetComponentInParent<LookAtTargetHandler>(); // LookAtTargetHandler 컴포넌트 가져오기
        currentHealth = maxHealth;  // 체력 초기화
        FindTarget();
    }

    void Update()
    {
        if (target == null) FindTarget();
        if (target == null) return;

        lookAtHandler.SetTarget(target);

        float distanceToTargetEdge = GetDistanceToTargetEdge();

        if (distanceToTargetEdge > attackRange)
        {
            MoveTowardsTarget();
        }
        else
        {
            Attack();                        // 서브클래스에서 구현
        }
    }

    // 타겟 찾기 (포탑/우주선)
    public void FindTarget()
    {
        GameObject[] candidates = null;

        switch (targetType)
        {
            case TargetType.ClosestTurret:
                candidates = GameObject.FindGameObjectsWithTag("Turret");
                break;
            case TargetType.BuffTurret:
                candidates = GameObject.FindGameObjectsWithTag("BuffTurret");
                break;
            case TargetType.Spaceship:
                GameObject ship = GameObject.FindGameObjectWithTag("Spaceship");
                if (ship != null) target = ship.transform;
                return;
        }

        float shortest = Mathf.Infinity;
        Transform nearest = null;

        foreach (var obj in candidates)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);
            if (d < shortest)
            {
                shortest = d;
                nearest = obj.transform;
            }
        }

        target = nearest;
    }

    // 타겟 방향으로 이동
    protected void MoveTowardsTarget()
    {
        Vector3 dir = (target.position - transform.parent.position).normalized;
        transform.parent.position += dir * speed * Time.deltaTime;
    }

    // 공격 로직 (서브클래스 구현)
    protected virtual void Attack()
    {
        if (damageCoroutine == null && target != null)
        {
            var targetDmg = target.GetComponent<IDamageable>();
            if (targetDmg != null)
            {
                damageCoroutine = StartCoroutine(DealDamageOverTime(targetDmg));
            }
        }
    }

    // IDamageable 구현: 피해 받기
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // 사망 처리
    protected void Die()
    {
        Destroy(transform.parent.gameObject);
    }


    // 충돌 시 피해 주기 (붙어서 공격)
    //void OnCollisionEnter(Collision collision)
    //{
    //    var otherFaction = collision.gameObject.GetComponent<FactionHandler>();
    //    if (otherFaction != null && otherFaction.faction != GetComponent<FactionHandler>().faction)
    //    {
    //        var dmg = collision.gameObject.GetComponent<IDamageable>();
    //        if (dmg != null && damageCoroutine == null)
    //            damageCoroutine = StartCoroutine(DealDamageOverTime(dmg));
    //    }
    //}

    //void OnCollisionExit(Collision collision)
    //{
    //    var otherFaction = collision.gameObject.GetComponent<FactionHandler>();
    //    if (otherFaction != null && otherFaction.faction != GetComponent<FactionHandler>().faction)
    //    {
    //        if (damageCoroutine != null)
    //        {
    //            StopCoroutine(damageCoroutine);
    //            damageCoroutine = null;
    //        }
    //    }
    //}

    IEnumerator DealDamageOverTime(IDamageable dmg)
    {
        while (dmg != null && (dmg as MonoBehaviour) != null)
        {
            dmg.TakeDamage(damage);
            yield return new WaitForSeconds(attackSpeed);
        }

        // 파괴되었으면 코루틴 정지
        damageCoroutine = null;
    }

    protected float GetDistanceToTargetEdge()
    {
        if (target == null) return Mathf.Infinity;

        Collider targetCol = target.GetComponent<Collider>();
        if (targetCol == null) return Vector3.Distance(transform.position, target.position);

        // 현재 위치에서 타겟 콜라이더 외곽에서 가장 가까운 지점 계산
        Vector3 closestPoint = targetCol.ClosestPoint(transform.position);
        float dist = Vector3.Distance(transform.position, closestPoint);

        return dist;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

