using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingEnemy : Enemy
{
    public float hackRange = 5f;                 // 해킹 범위
    public float hackDuration = 5f;               // 해킹 지속 시간
    public GameObject hackExplosionEffect;   // 해킹 폭발 이펙트(옵션)
    

    protected override void Start()
    {
        base.Start();
    }

    

    protected override void Attack()
    {
        if (target != null)
        {
            float dist = GetDistanceToTargetEdge();
            if (dist <= attackRange)
            {
                ApplyHackToNearbyTurrets();
                Die();
            }
        }
    }

    private void ApplyHackToNearbyTurrets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hackRange);
        foreach (var hit in hits)
        {
            Turret turret = hit.GetComponent<Turret>();
            if (turret != null)
            {
                turret.SetHacked(true, hackDuration);
            }
        }
    }


    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, hackRange);
    }
}
