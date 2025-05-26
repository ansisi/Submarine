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
    

    [SerializeField]
    private float currentDurability;
    private float nextFireTime = 0f;
    private Transform target;
    private LookAtTargetHandler lookAtHandler;
    
    //EMP 드론 관련
    private HashSet<EMPEnemy> empSources = new HashSet<EMPEnemy>(); // 여러 EMPEnemy 출처를 관리하기 위한 집합
    private Renderer[] turretRenderers;
    private Color[] originalColors;
    private bool isEMP = false; // EMP 상태 별도 관리

    //해킹 드론 관련
    private bool isHacked = false;
    private float hackDurationTimer = 0f; // 해킹 지속시간
    [SerializeField] 
    private float hackCooldown = 10f; // 개인 해킹 쿨타임
    private float hackCooldownTimer = 0f; // 현재 쿨타임 카운터
    

    // 해킹 상태일 때 공격할 대상 태그 목록 우선순위
    private readonly string[] hackedTargetPriorityTags = new string[] { "Turret", "BuffTurret", "Spaceship" };
    public bool IsHacked => isHacked;





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
            hackCooldownTimer -= Time.deltaTime;

        if (isHacked)
        {
            if (hackDurationTimer > 0f)
                hackDurationTimer -= Time.deltaTime;

            if (hackDurationTimer <= 0f)
            {
                SetHacked(false);
            }
        }

        if (currentDurability <= 0) return;

        if (isHacked)
        {
            FindHackedTarget();
            if (target != null)
            {
                lookAtHandler.SetTarget(target);
                if (Vector3.Distance(transform.position, target.position) <= range)
                {
                    if (Time.time >= nextFireTime)
                    {
                        Fire();
                        nextFireTime = Time.time + 1f / fireRate;
                    }
                }
            }
            // 해킹 상태라 회전은 활성화되어 있음

            return;
        }

        if (isDisabled) // EMP 상태에서는 공격 및 회전 중단
        {
            return;
        }

        // 정상 상태 공격 로직
        FindTarget();
        if (target != null)
        {
            lookAtHandler.SetTarget(target);
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
                bulletScript.Initialize(direction, GetComponent<Collider>()); // Bullet의 방향 설정
            }

            // 총알 발사 방향을 선으로 시각화
            Debug.DrawRay(firePoint.position, direction * 2f, Color.red);
        }
    }

    // EMP 출처 추가 (EMPEnemy가 비활성화 신호 보낼 때 호출)
    public void AddEMPSource(EMPEnemy source)
    {
        if (source == null) return; // 방어코드
        empSources.Add(source);
        UpdateEMPStatus();
    }

    // EMP 출처 제거 (EMPEnemy가 범위 벗어나거나 죽을 때 호출)
    public void RemoveEMPSource(EMPEnemy source)
    {
        if (this == null) return; // Turret이 파괴됐으면 무시

        if (empSources.Remove(source))
        {
            UpdateEMPStatus();
        }
    }

    // EMP 출처 개수에 따라 비활성화 상태 변경
    private void UpdateEMPStatus()
    {
        // 파괴된 EMPEnemy 객체 제거 (null 검사)
        empSources.RemoveWhere(source => source == null);
        isEMP = empSources.Count > 0 && !isHacked; // EMP 활성화 조건: EMP 신호 있고 해킹 중 아님

        UpdateTurretState(); // EMP 상태 변경 후 최종 상태 갱신
    }

    // 실제 비활성화 처리
    private void DisableTurret()
    {
        if (this == null) return; // 오브젝트가 파괴되었으면 함수 종료

        Logger.Log($"{name} - EMP에 의해 비활성화됨");
        //이펙트, 사운드 재생 등 추가 처리 가능

        // LookAtTargetHandler 비활성화
        if (lookAtHandler != null)
            lookAtHandler.enabled = false;

        ResetTurretColor(); // 색상 복원 
    }

    // 실제 활성화 처리
    private void EnableTurret()
    {
        if (this == null) return; // 오브젝트가 파괴되었으면 함수 종료

        Logger.Log($"{name} - EMP 해제되어 활성화됨");
        // 이펙트 정지, 상태 복구 등 추가 처리 가능

        // LookAtTargetHandler 다시 활성화
        if (lookAtHandler != null)
            lookAtHandler.enabled = true;

        ResetTurretColor(); // 색상 복원
    }

    public void SetHacked(bool hacked, float hackDuration = 5f)
    {
        if (this == null || this.gameObject == null)
        {
            // 이미 파괴된 상태면 함수 조기 종료
            return;
        }
        if (hacked)
        {

            if (isHacked)  // 이미 해킹 상태면 중복 무시
                return;

            if (hackCooldownTimer <= 0f && !isHacked)  // 쿨타임 끝났고 해킹 중이 아니면 허용
            {
                isHacked = true;
                hackDurationTimer = hackDuration;
                OnHackedStart();

                // EMP 상태가 있으면 해킹 우선. EMP는 무효화 상태로 변경(해킹이 우선)
                if (isEMP)
                {
                    isEMP = false;
                    Logger.Log($"{name} - EMP 해제, 해킹 우선 적용");
                }
            }
        }
        else
        {
            if (isHacked) // 해킹 종료 시에만 처리
            {
                isHacked = false;
                hackDurationTimer = 0;
                hackCooldownTimer = hackCooldown; // 해킹 쿨타임 시작
                OnHackedEnd();

                // 해킹 끝났는데 EMP 신호가 여전히 있다면 EMP 상태 활성화
                if (empSources.Count > 0)
                {
                    isEMP = true;
                    Logger.Log($"{name} - 해킹 해제 후 EMP 상태 전환");
                }
            }
        }
        UpdateTurretState();
    }

    private void OnHackedStart()
    {
        ResetTurretColor(); // 색상 복원 대신 상태 기반 갱신

        // 추가로 해킹 시 동작 변경 로직 가능
    }

    private void OnHackedEnd()
    {
        ResetTurretColor(); // 색상 복원 대신 상태 기반 갱신

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
                if (candidate == this.gameObject || !candidate.activeInHierarchy) continue;

                float dist = Vector3.Distance(transform.position, candidate.transform.position);
                if (dist < range && dist < shortestDist)  // 사거리 내인지 검사 추가
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

    private void UpdateTurretState()
    {
        if (this == null) return; // 오브젝트가 파괴되었으면 함수 종료

        // 상태 우선순위: 해킹 > EMP > 정상
        if (isHacked)
        {
            if (isDisabled)  // 만약 비활성 상태라면 활성화 처리
            {
                isDisabled = false;
                EnableTurret();  // 터렛 기능 활성화
            }
            lookAtHandler.enabled = true;  // 해킹 시 조준기능 켜기
            UpdateTurretColor(Color.red);  // 해킹 상태 컬러
        }
        else if (isEMP)
        {
            if (!isDisabled)  // EMP인데 아직 활성 상태면 비활성화 처리
            {
                isDisabled = true;
                DisableTurret();  // 터렛 기능 비활성화
            }
            UpdateTurretColor(new Color(0f, 0.4f, 0.5f, 1f));  // EMP 색상
        }
        else
        {
            if (isDisabled)  // 일반 상태인데 비활성화면 활성화 처리
            {
                isDisabled = false;
                EnableTurret();
            }
            lookAtHandler.enabled = true;  // 정상 상태, 조준 활성화
            ResetTurretColor();  // 기본 색상
        }
    }

    private void UpdateTurretColor(Color color)
    {
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            if (turretRenderers[i] != null)
            {
                turretRenderers[i].material.color = color;
            }
        }
    }

    private void ResetTurretColor()
    {
        for (int i = 0; i < turretRenderers.Length; i++)
        {
            if (turretRenderers[i] != null)
            {
                turretRenderers[i].material.color = originalColors[i];
            }
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
