using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : DeliverableItem
{
    [Header("Explosion Settings")]
    public float explosionDelay = 3f;     // 잡힌 후 3초 뒤에 폭발
    public float explosionForce = 500f;   // 폭발 힘
    public float explosionRadius = 5f;    // 폭발 범위
    public float upwardModifier = 1f;     // 위로 밀어올리는 보정값

    [Header("Damage Settings")]
    public float oxygenDamage = -20f;      // 플레이어에게 줄 산소 게이지 감소량

    [Header("Layer Settings")]
    public LayerMask affectedLayers;      // 폭발에 영향을 받을 레이어 (예: 플레이어, 적, 물리 오브젝트)

    private bool hasExploded = false;     // 중복 폭발 방지를 위한 변수

    private Renderer[] mineRenderers;     // 자식들에 있는 모든 Renderer
    private Color[] originalColors;       // 각 Renderer의 원래 색상

    public override void OnGrabbed()
    {
        base.OnGrabbed();

        // 모든 자식 Renderer 컴포넌트를 가져옴
        mineRenderers = GetComponentsInChildren<Renderer>();
        if (mineRenderers != null && mineRenderers.Length > 0)
        {
            originalColors = new Color[mineRenderers.Length];
            for (int i = 0; i < mineRenderers.Length; i++)
            {
                originalColors[i] = mineRenderers[i].material.color;
            }
        }

        // 잡히면 폭발 카운트다운 시작
        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        float elapsed = 0f;
        bool flashState = false;
        float flashInterval = 1f;

        // 지정된 시간 동안 1초마다 색상을 토글하여 경고 효과 적용
        while (elapsed < explosionDelay)
        {
            flashState = !flashState;
            if (mineRenderers != null)
            {
                for (int i = 0; i < mineRenderers.Length; i++)
                {
                    if (mineRenderers[i] != null)
                    {
                        mineRenderers[i].material.color = flashState ? Color.red : originalColors[i];
                    }
                }
            }
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // 폭발 직전에 모든 Renderer의 색상을 원래대로 복원
        if (mineRenderers != null)
        {
            for (int i = 0; i < mineRenderers.Length; i++)
            {
                if (mineRenderers[i] != null)
                {
                    mineRenderers[i].material.color = originalColors[i];
                }
            }
        }

        Explode();
    }

    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // (옵션) 폭발 이펙트나 사운드를 재생하는 코드를 여기에 추가할 수 있습니다.
        // 예: Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // 폭발 범위 내의 콜라이더 검색 (affectedLayers에 포함된 오브젝트만 검색)
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, affectedLayers);

        foreach (Collider hit in colliders)
        {
            // Rigidbody가 있는 경우 폭발 힘을 가함
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
            }

            // 플레이어인지 확인 후 산소 게이지 감소 처리
            // 이 예제에서는 PlayerStatus라는 스크립트가 산소를 관리한다고 가정합니다.
            OxygenTank oxygenTank = hit.GetComponent<OxygenTank>();
            if (oxygenTank != null)
            {
                oxygenTank.AddOxygen(oxygenDamage);
            }
        }

        // 아이템을 놓는 처리 (DeliverableItem의 Release 메서드 호출)
        Release();

        // 폭발 후 자기 자신 제거
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // 폭발 범위를 나타내는 색 지정 (예: 빨간색)
        Gizmos.color = Color.red;
        // 현재 위치를 중심으로 explosionRadius 크기의 와이어 구를 그림
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // DeliverableItem에서 추상 메서드 OnDelivered 구현 (필요 시 내용 추가)
    public override void OnDelivered(Submarine submarine)
    {
        // Mine은 전달되면 별도의 동작이 없거나 파괴될 수 있음.
    }
}
