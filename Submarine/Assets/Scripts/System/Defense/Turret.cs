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

    [SerializeField]
    private float currentDurability;
    private float nextFireTime = 0f;
    private Transform target;
    private LookAtTargetHandler lookAtHandler;

    void Start()
    {
        lookAtHandler = GetComponent<LookAtTargetHandler>(); // LookAtTargetHandler 컴포넌트 가져오기
        currentDurability = maxDurability;
    }

    void Update()
    {
        if (currentDurability <= 0) return;

        FindTarget();

        // 목표가 존재하면 회전
        if(target != null)
        {
            lookAtHandler.SetTarget(target);  // 타겟 설정

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
            // 적의 콜라이더가 자식 오브젝트에 있을 경우에도 찾아야 하므로, hit.transform을 확인
            if (hit.transform != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearest = hit.transform;
                }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
