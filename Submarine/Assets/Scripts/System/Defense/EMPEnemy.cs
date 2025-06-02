using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPEnemy : Enemy
{
    public float empRange = 5f;  // EMP 범위
    public float empDuration = 5f;  // EMP 효과 지속 시간
    public GameObject empExplosionEffect; // EMP 폭발 이펙트 (옵션)

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
                ApplyEMPToNearbyTurrets();
                Die();
            }
        }
    }

    private void ApplyEMPToNearbyTurrets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, empRange);
        foreach (var hit in hits)
        {
            Turret turret = hit.GetComponent<Turret>();
            if (turret != null)
            {
                turret.ApplyEMPEffect(empDuration);
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, empRange);
    }
}
