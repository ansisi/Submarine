using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingEnemy : Enemy
{
    public float hackRange = 5f;                 // 해킹 범위
    public float checkInterval = 0.2f;           // 해킹 검사 주기(초)
    public float hackDuration = 5f;               // 해킹 지속 시간

    private HashSet<Turret> turretsInRange = new HashSet<Turret>();   // 해킹 중인 포탑들
    private Coroutine checkCoroutine;

    protected override void Start()
    {
        base.Start();
        checkCoroutine = StartCoroutine(CheckHackRangeLoop());
    }

    private IEnumerator CheckHackRangeLoop()
    {
        while (true)
        {
            CheckHackRange();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckHackRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hackRange);
        HashSet<Turret> currentTurrets = new HashSet<Turret>();

        foreach (var hit in hits)
        {
            Turret turret = hit.GetComponent<Turret>();
            if (turret != null)
            {
                currentTurrets.Add(turret);

                if (!turretsInRange.Contains(turret))
                {
                    turretsInRange.Add(turret);
                    turret.SetHacked(true, hackDuration);  // 해킹 시작
                }
            }
        }

        // 해킹 범위를 벗어난 포탑은 해킹 해제
        var removedTurrets = new List<Turret>();
        foreach (var turret in turretsInRange)
        {
            if (!currentTurrets.Contains(turret) || !turret.IsHacked)
            {
                turret.SetHacked(false);
                removedTurrets.Add(turret);
            }
        }

        foreach (var turret in removedTurrets)
        {
            turretsInRange.Remove(turret);
        }
    }

    protected override void Attack()
    {
        // 해킹 드론은 별도 공격 없음 (필요 시 구현 가능)
    }

    private void OnDestroy()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }

        // 드론이 파괴되면 해킹 해제
        foreach (var turret in turretsInRange)
        {
            if (turret != null)
                turret.SetHacked(false);
        }
        turretsInRange.Clear();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, hackRange);
    }
}
