using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float explosionDelay = 1f; // 터지기까지 대기 시간
    public float explosionRadius = 5f; // 폭발 반경
    public float explosionDamage = 30f; // 폭발 데미지
    public float explosionForce = 500f; // 플레이어를 밀어낼 힘

    private bool isTriggered = false; // 이미 트리거됐는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        // 적인지 확인
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            isTriggered = true;
            StartCoroutine(ExplodeAfterDelay());
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);

        // 폭발 처리
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }


            // 플레이어 날아가게 처리
            if (hitCollider.CompareTag("Player"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }

        // 폭발 이펙트 추가 가능
        // 예: Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject); // 폭탄 삭제
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

