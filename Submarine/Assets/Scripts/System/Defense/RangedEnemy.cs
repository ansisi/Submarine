using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public GameObject bulletPrefab;         // 총알 프리팹
    public Transform firePoint;             // 총알 발사 위치
    public float attackCooldown = 1.5f;     // 공격 쿨타임

    private float lastAttackTime;           // 마지막 공격 시간 기록

    protected override void Attack()
    {
        // 쿨타임 확인
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (bulletPrefab == null || firePoint == null || target == null) return;

        // 공격 사정거리 안에 들어왔다면 공격
        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // 목표 방향 계산 (target - firePoint)
            Vector3 direction = (target.position - firePoint.position).normalized;

            // Bullet 스크립트에서 총알의 방향 설정
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.faction = GetComponent<FactionHandler>().faction; // 진영 설정
                bulletScript.direction = direction; // 총알 방향 설정
            }

            lastAttackTime = Time.time;
        }
    }

    void Update()
    {

        if (target == null)
        {
            FindTarget();
            if (target == null) return;
        }

        // 목표를 쳐다보는 코드
        lookAtHandler.SetTarget(target);  // 타겟 설정

        // 사정거리 내외를 구분하여 공격
        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            Attack();
        }
        else
        {
            // 목표를 향해 이동
            MoveTowardsTarget();
        }
        
    }
}
