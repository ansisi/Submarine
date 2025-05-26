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
    public GameObject hackedBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float range = 5f;
    public float maxDurability = 100f;
    public LayerMask enemyLayer;
    public bool isDisabled { get; private set; } = false; // EMP 상태
    public bool isHacked { get; private set; } = false; // 해킹 상태

    [SerializeField]
    private float currentDurability;
    private float nextFireTime = 0f;
    private Transform target;
    private LookAtTargetHandler lookAtHandler;
    
    //EMP 드론 관련
    private HashSet<EMPEnemy> empSources = new HashSet<EMPEnemy>(); // 여러 EMPEnemy 출처를 관리하기 위한 집합
    private Renderer[] turretRenderers;
    private Color[] originalColors;

    //해킹 드론 관련
    private GameObject hackedTarget;
    private float hackCooldownTimer = 0f; // 해킹 쿨타임 관리용

    // 해킹 상태일 때 공격할 대상 태그 목록 우선순위
    private readonly string[] hackedTargetPriorityTags = new string[] { "Turret", "BuffTurret", "Spaceship" };

    public bool IsHackable => !isHacked && hackCooldownTimer <= 0f;



    void Awake()
    {
        turretRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[turretRenderers.Length];

        for (int i = 0; i < turretRenderers.Length; i++)
        {
            originalColors[i] = turretRenderers[i].material.color;
        }
    }

    void Start()
    {
        lookAtHandler = GetComponent<LookAtTargetHandler>(); // LookAtTargetHandler 컴포넌트 가져오기
        currentDurability = maxDurability;
    }

    void Update()
    {
        if (hackCooldownTimer > 0f)
        {
            hackCooldownTimer -= Time.deltaTime;
            if (hackCooldownTimer <= 0f && isHacked)
            {
                SetHacked(false);  // 쿨타임 끝나면 해킹 해제
            }
        }

        if (currentDurability <= 0 || isDisabled) return;

        if (isHacked)
        {
            FindHackedTarget();  // 해킹 상태일 때는 아군 공격 목표 찾기
        }
        else
        {
            FindTarget();        // 정상 상태일 때는 적 공격 목표 찾기
        }

        // 목표가 존재하면 회전
        if (target != null)
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
        if (firePoint != null && target != null)
        {
            // 목표를 향한 방향 계산
            Vector3 direction = (target.position - firePoint.position).normalized;

            GameObject bulletToSpawn = bulletPrefab; // 기본 총알 프리팹

            if (isHacked)
            {
                bulletToSpawn = hackedBulletPrefab; // 해킹 상태일 때는 적 총알 프리팹
            }

            GameObject bullet = Instantiate(bulletToSpawn, firePoint.position, firePoint.rotation);

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

    // EMP 출처 추가 (EMPEnemy가 비활성화 신호 보낼 때 호출)
    public void AddEMPSource(EMPEnemy source)
    {
        if (empSources.Add(source)) // 새로 추가되면 상태 업데이트
        {
            UpdateEMPStatus();
        }
    }

    // EMP 출처 제거 (EMPEnemy가 범위 벗어나거나 죽을 때 호출)
    public void RemoveEMPSource(EMPEnemy source)
    {
        if (empSources.Remove(source))
        {
            UpdateEMPStatus();
        }
    }

    // EMP 출처 개수에 따라 비활성화 상태 변경
    private void UpdateEMPStatus()
    {
        bool shouldDisable = empSources.Count > 0;

        if (shouldDisable != isDisabled)
        {
            isDisabled = shouldDisable;
            if (isDisabled) DisableTurret();
            else EnableTurret();
        }
    }

    // 실제 비활성화 처리
    private void DisableTurret()
    {
        Logger.Log($"{name} - EMP에 의해 비활성화됨");
        //이펙트, 사운드 재생 등 추가 처리 가능

        // LookAtTargetHandler 비활성화
        if (lookAtHandler != null)
            lookAtHandler.enabled = false;

        // 색상 변경 
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            turretRenderers[i].material.color = new Color(0f, 0.4f, 0.5f, 1f);
        }
    }

    // 실제 활성화 처리
    private void EnableTurret()
    {
        Logger.Log($"{name} - EMP 해제되어 활성화됨");
        // 이펙트 정지, 상태 복구 등 추가 처리 가능

        // LookAtTargetHandler 다시 활성화
        if (lookAtHandler != null)
            lookAtHandler.enabled = true;

        // 색상 원래대로 복원
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            turretRenderers[i].material.color = originalColors[i];
        }
    }

    public void SetHacked(bool hacked, float hackCooldown = 5f)
    {
        if (hacked)
        {
            isHacked = true;
            hackCooldownTimer = hackCooldown;
            OnHackedStart();
        }
        else
        {
            isHacked = false;
            OnHackedEnd();
        }
    }

    private void OnHackedStart()
    {
        // 해킹 시작 시 처리 (예: 색상 변경)
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            turretRenderers[i].material.color = Color.red;  // 해킹 중 색상 예시
        }

        // 추가로 해킹 시 동작 변경 로직 가능
    }

    private void OnHackedEnd()
    {
        // 해킹 종료 시 처리 (색상 복구)
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            turretRenderers[i].material.color = originalColors[i];
        }

        // 추가로 해킹 해제 시 동작 복구 로직 가능
    }

    private void FindHackedTarget()
    {
        float shortestDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (var tag in hackedTargetPriorityTags)
        {
            GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
            foreach (var candidate in candidates)
            {
                // 자신(해킹당한 포탑) 제외
                if (candidate == this.gameObject) continue;

                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearest = candidate.transform;
                }
            }
            if (nearest != null)
            {
                // 현재 우선순위 그룹에서 목표 찾았으면 종료
                break;
            }
        }
        target = nearest;
    }

    public void TakeDamage(float amount)
    {
        currentDurability -= amount;
        if (currentDurability <= 0f)
        {
            Die();
        }
    }

    public float CurrentDurabilityRatio()
    {
        return Mathf.Clamp01(currentDurability / maxDurability);
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
