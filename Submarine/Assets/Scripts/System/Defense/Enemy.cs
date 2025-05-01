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
    public float rotationSpeed = 360f;      // 회전 속도
    public Transform modelTransform; // 모델 자식 참조

    protected float currentHealth;           // 현재 체력
    protected Transform target;              // 현재 타겟
    protected Coroutine damageCoroutine;

    void Start()
    {
        currentHealth = maxHealth;           // 체력 초기화
        FindTarget();
    }

    void Update()
    {
        if (target == null) FindTarget();
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        LookAtTarget();

        if (dist > attackRange)
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
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    // 공격 로직 (서브클래스 구현)
    protected virtual void Attack()
    {
        // 기본 구현은 없음
    }

    // IDamageable 구현: 피해 받기
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    // 사망 처리
    void Die()
    {
        Destroy(gameObject);
    }

    // 목표를 쳐다보는 함수
    public void LookAtTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.z = 0f; // 상하 방향 제거 → 수평 방향만 유지

        if (dir == Vector3.zero) return; // 타겟이 본체와 정확히 일치하는 경우를 방지

        // Atan2로 회전 각도 계산
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (dir.x < 0f)
            angle += 180f;

        // 본체 오브젝트만 Z축으로 회전하도록 설정
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, angle), rotationSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 360f);

        // 모델 기울기 처리 (자식)
        if (modelTransform != null)
        {
            if (dir.x < 0f)
                modelTransform.localRotation = Quaternion.Euler(40f, 180f, 0f); // 왼쪽
            else
                modelTransform.localRotation = Quaternion.Euler(-40f, 0f, 0f);  // 오른쪽
        }
    }

    // 충돌 시 피해 주기 (붙어서 공격)
    void OnCollisionEnter(Collision collision)
    {
        var otherFaction = collision.gameObject.GetComponent<FactionHandler>();
        if (otherFaction != null && otherFaction.faction != GetComponent<FactionHandler>().faction)
        {
            var dmg = collision.gameObject.GetComponent<IDamageable>();
            if (dmg != null && damageCoroutine == null)
                damageCoroutine = StartCoroutine(DealDamageOverTime(dmg));
        }
    }

    void OnCollisionExit(Collision collision)
    {
        var otherFaction = collision.gameObject.GetComponent<FactionHandler>();
        if (otherFaction != null && otherFaction.faction != GetComponent<FactionHandler>().faction)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    IEnumerator DealDamageOverTime(IDamageable dmg)
    {
        while (dmg != null && (dmg as MonoBehaviour) != null)
        {
            dmg.TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }

        // 파괴되었으면 코루틴 정지
        damageCoroutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

