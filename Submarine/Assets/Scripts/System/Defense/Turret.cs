using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
}

[RequireComponent(typeof(FactionHandler))]
public class Turret : MonoBehaviour, IDamageable
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float range = 5f;
    public float maxDurability = 100f;
    public LayerMask enemyLayer;
    public float rotationSpeed = 360f; // 터렛 회전 속도
    public Transform modelTransform;

    [SerializeField]
    private float currentDurability;
    private float nextFireTime = 0f;
    private Transform target;

    void Start()
    {
        currentDurability = maxDurability;
    }

    void Update()
    {
        if (currentDurability <= 0) return;

        FindTarget();

        // 목표가 존재하면 회전
        if(target != null)
        {
            LookAtTarget();

            // 사정거리 내에서만 공격
            if (Vector3.Distance(transform.position, target.position) <= range)
            {
                if (Time.time >= nextFireTime)
                {
                    Fire();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        float shortestDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearest = hit.transform;
            }
        }

        target = nearest;
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null && target != null)
        {
            // 목표를 향한 방향 계산
            Vector3 direction = (target.position - firePoint.position).normalized;

            // 총알 생성, 방향을 target을 향하도록 설정
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 총알의 방향 설정
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.direction = direction; // Bullet의 방향 설정
            }

            // 총알 발사 방향을 선으로 시각화
            Debug.DrawRay(firePoint.position, direction * 2f, Color.red);
        }
    }

    public void TakeDamage(float amount)
    {
        currentDurability -= amount;
        if (currentDurability <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void LookAtTarget()
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, 0f, angle), rotationSpeed * Time.deltaTime);
        

        // 모델 기울기 처리 (자식)
        if (modelTransform != null)
        {
            if (dir.x < 0f)
                modelTransform.localRotation = Quaternion.Euler(0f, 180f, 0f); // 왼쪽
            else
                modelTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);  // 오른쪽
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
