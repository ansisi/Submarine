using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(FactionHandler))]
public class LaserTurret : Turret
{
    [Header("레이저용 설정")]
    [SerializeField] private float laserDamagePerSecond = 20f; // 초당 레이저 데미지
    [SerializeField] private float damageDelay = 1f;          // 데미지만 지연
    private FactionHandler myFaction;   // turret의 진영 정보
    private float damageTimer = 0f;

    [SerializeField] private VisualEffect laserVFX; // 레이저 효과
    [ColorUsage(false, true)]
    [SerializeField] private Color[] normalColors = new Color[5];
    [ColorUsage(false, true)]
    [SerializeField] private Color[] hackedColors = new Color[5];
    

    protected override void Start()
    {
        base.Start();
        myFaction = GetComponent<FactionHandler>();
        UpdateVFXColor(); // 초기 VFX 색상 설정
    }

    void Update()
    {
        // EMP 상태면 무조건 꺼두고 아무것도 안 함
        if (isEMPDisabled)
        {
            DisableLaser();
            return;
        }

        if (isHacked) HandleHackTimer(); // 해킹 타이머만 처리

        if (currentDurability <= 0f)
        {
            DisableLaser(); return;
        }

        if (isHacked)
            FindHackedTarget();
        else
            FindTarget();

        if (target != null && Vector3.Distance(transform.position, target.position) <= range)
        {
            lookAtHandler.SetTarget(target);

            
            // VFX는 즉시 켜기
            if (laserVFX != null)
                laserVFX.SetBool("Enabled", true);

            // 데미지 타이머 누적
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageDelay)
                DealLaserDamage(); 
        }
        else
        {
            DisableLaser(); // 사거리 벗어나면 끄기
            damageTimer = 0f;  // 초기화
        }
    }

    
    private void DealLaserDamage()
    {
        Vector3 origin = firePoint.position;
        Vector3 dir = firePoint.right;
        Debug.DrawRay(origin, dir * range, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(origin, dir, range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance)); // 가까운 순 정렬

        foreach (var hit in hits)
        {
            var otherFaction = hit.collider.GetComponent<FactionHandler>();
            if (otherFaction == null) continue;

            bool shouldDamage = !isHacked
                ? myFaction.IsEnemy(otherFaction)
                : myFaction.IsAlly(otherFaction);

            if (shouldDamage)
            {
                var dmg = hit.collider.GetComponent<IDamageable>();  // 데미지 가능한지 확인
                if (dmg != null)
                    dmg.TakeDamage(laserDamagePerSecond * Time.deltaTime);  // 초당 데미지
            }
        }
    }

    // 해킹 시작 시 VFX 컬러 즉시 변경
    protected override void OnHackedStart()
    {
        base.OnHackedStart();
        UpdateVFXColor();
    }

    // 해킹 종료 시 원래 컬러로 복원
    protected override void OnHackedEnd()
    {
        base.OnHackedEnd();
        UpdateVFXColor();
    }

    // EMP 시작/끝에도 VFX 색 갱신
    public override void ApplyEMPEffect(float duration)
    {
        base.ApplyEMPEffect(duration);
        DisableLaser();
        // UpdateVFXColor();
    }

    private void UpdateVFXColor()
    {
        if (laserVFX == null) return;
        for (int i = 0; i < 5; i++)
        {
            string propName = $"BeamColor_Part{i}";
            Color col = isHacked
                ? hackedColors[i]
                : normalColors[i];
            laserVFX.SetVector4(propName, col.linear);
        }
    }

    private void DisableLaser()
    {
        if (laserVFX != null)
            laserVFX.SetBool("Enabled", false); 
    }

    private void HandleHackTimer()
    {
        if (hackDurationTimer > 0f)
            hackDurationTimer -= Time.deltaTime;

        if (hackDurationTimer <= 0f)
            SetHacked(false);
    }
}
