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
    private bool isEMPDisabled = false;      // 현재 EMP로 인해 비활성화되었는지
    private Renderer[] turretRenderers;
    private Color[] originalColors;

    //해킹 드론 관련
    private bool isHacked = false;
    private float hackDurationTimer = 0f; // 해킹 지속시간

    // 해킹 상태일 때 공격할 대상 태그 목록 우선순위
    private readonly string[] hackedTargetPriorityTags = new string[] { "Turret", "BuffTurret", "Spaceship" };
    





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
        if (isEMPDisabled)
        {
            // EMP 상태에서는 아무것도 못 함
            return;
        }

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

    // 실제 비활성화 처리
    private void DisableTurret()
    {
        if (this == null) return; // 오브젝트가 파괴되었으면 함수 종료

        Logger.Log($"{name} - EMP에 의해 비활성화됨");
        //이펙트, 사운드 재생 등 추가 처리 가능

        // LookAtTargetHandler 비활성화
        if (lookAtHandler != null)
            lookAtHandler.enabled = false;

        // EMP 전용 색상 (기존: Cyan 계열)
        UpdateTurretColor(new Color(0f, 0.4f, 0.5f, 1f));
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

    public void ApplyEMPEffect(float duration)
    {
        // 이미 해킹 중이면 EMP 무시
        if (isHacked)
            return;

        // 중복 호출 방지
        if (isEMPDisabled)
            return;

        StartCoroutine(EMPDisableCoroutine(duration));
    }

    private IEnumerator EMPDisableCoroutine(float duration)
    {
        // EMP 시작: 컬러 변경 + 컴포넌트 비활성화
        isEMPDisabled = true;
        isDisabled = true;  // public bool isDisabled 플래그 유지용
        DisableTurret();    // 기존 DisableTurret() 호출해서 회전/사격 중단 + 컬러 변경

        // duration 초 동안 대기
        yield return new WaitForSeconds(duration);

        // EMP 종료: 원래 상태 복귀
        isEMPDisabled = false;
        isDisabled = false;
        EnableTurret();   // 기존 EnableTurret() 호출해서 정상 상태로 돌아오기
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

            if (isDisabled || isEMPDisabled)
            {
                // EMP DisableCoroutine이 돌고 있는 중이면 곧바로 중단
                isEMPDisabled = false;
                isDisabled = false;
                EnableTurret();
            }

            isHacked = true;
            hackDurationTimer = hackDuration;
            OnHackedStart();

        }
        else
        {
            if (isHacked) // 해킹 종료 시에만 처리
            {
                isHacked = false;
                hackDurationTimer = 0;
                
                OnHackedEnd();

                // 해킹 끝났는데 EMP 신호가 여전히 있다면 EMP 상태 활성화
                
            }
        }
        UpdateTurretState();
    }

    private void OnHackedStart()
    {
        UpdateTurretColor(Color.red);

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
