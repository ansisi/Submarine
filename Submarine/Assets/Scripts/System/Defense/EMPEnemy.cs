using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPEnemy : Enemy
{
    public float empRange = 5f;  // EMP 범위
    public float checkInterval = 0.2f;  // 검사 주기 (초)

    private HashSet<Turret> turretsInRange = new HashSet<Turret>();  // 현재 EMP 범위 내 포탑들
    private Coroutine checkCoroutine;

    protected override void Start()
    {
        base.Start(); 
        checkCoroutine = StartCoroutine(CheckEMPRangeLoop());
    }

    private IEnumerator CheckEMPRangeLoop()
    {
        while (true)
        {
            CheckEMPRange();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckEMPRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, empRange);
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
                    turret.AddEMPSource(this);
                }
            }
        }

        var removedTurrets = new List<Turret>();
        foreach (var turret in turretsInRange)
        {
            if (!currentTurrets.Contains(turret))
            {
                turret.RemoveEMPSource(this);
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
        // EMPEnemy는 공격 안 함
    }

    private void OnDestroy()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }

        foreach (var turret in turretsInRange)
        {
            if (turret != null)
                turret.RemoveEMPSource(this);
        }
        turretsInRange.Clear();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, empRange);  // empRange 그대로 사용
    }
}
