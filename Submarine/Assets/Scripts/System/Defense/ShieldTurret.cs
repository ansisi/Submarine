using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FactionHandler))]
public class ShieldTurret : InteractableBase, IDamageable
{
    [Header("보호막 포탑 체력 설정")]
    [SerializeField] private float maxDurability = 200f;
    [SerializeField] private float currentDurability;

    [Header("보호막 반경")]
    [SerializeField] private float shieldRadius = 8f;

    [Header("버프 지속 시간")]
    [SerializeField] private float buffDuration = 15f;

    [Header("충전 중일 때 표시할 색상")]
    [SerializeField] private Color chargedColor = Color.green;

    [Header("버프 중일 때 표시할 색상")]
    [SerializeField] private Color buffingColor = Color.blue;

    [Header("언차지(비활성) 상태 색상")]
    [SerializeField] private Color unchargedColor = Color.gray;

    [Header("상호작용 힌트 텍스트")]
    [SerializeField] private string rechargeHint = "스페이스: 보호막 재충전";

    [SerializeField] private Sprite shieldTurretIcon;


    // 상태 머신: 충전됨 → 버프 중 → 언차지
    private enum ShieldState { Charged, Buffing, Uncharged }
    private ShieldState currentState = ShieldState.Charged;

    // 버프가 적용된 터렛들 (버프 종료 시 필요)
    private List<Turret> buffedTurrets = new List<Turret>();

    // 버프 지속 코루틴 참조
    private Coroutine buffCoroutine;

    // 시각용 Renderer 배열 (머테리얼 색상 변경용)
    private Renderer[] shieldRenderers;
    private Color[] originalColors;

    // 재충전 대기 중인지 여부
    private bool waitingForRecharge = false;

    private Transform playerTransform;

    private void Awake()
    {
        // 체력 초기화
        currentDurability = maxDurability;

        // Renderer 캐싱 (자식 포함)
        shieldRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[shieldRenderers.Length];
        for (int i = 0; i < shieldRenderers.Length; i++)
        {
            originalColors[i] = shieldRenderers[i].material.color;
        }
    }

    private void Start()
    {
        InteractionManager.instance.Register(this);

        // 플레이어 Transform 찾기
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
            playerTransform = playerGO.transform;

        // 처음에는 Charged 상태
        currentState = ShieldState.Charged;
        waitingForRecharge = false;
        SetStateVisual(currentState);
    }

    private void Update()
    {
        // 체력이 0 이하이면 더 이상 동작하지 않음
        if (currentDurability <= 0f) return;

        // 터렛 감지: Charged 상태일 때만 주변 터렛 모니터링
        if (currentState == ShieldState.Charged)
        {
            Collider[] hitsAll = Physics.OverlapSphere(transform.position, shieldRadius);
            foreach (var hit in hitsAll)
            {
                if (!hit.CompareTag("Turret"))
                    continue;

                Turret turret = hit.GetComponent<Turret>();
                if (turret == null)
                    continue;

                // 터렛이 EMP나 해킹 상태인지 체크
                if (turret.isDisabled || turret.IsCurrentlyHacked())
                {
                    ActivateShield();
                    return; // 한 번만 발동하면 충분
                }
            }
        }
    }

    /// <summary>
    /// 보호막 발동: 주변 터렛을 정화하고 면역 부여 → 버프 유지 코루틴 시작
    /// </summary>
    private void ActivateShield()
    {
        currentState = ShieldState.Buffing;
        SetStateVisual(currentState);

        buffedTurrets.Clear();

        Collider[] hitsAll = Physics.OverlapSphere(transform.position, shieldRadius);
        foreach (var hit in hitsAll)
        {
            if (!hit.CompareTag("Turret"))
                continue;

            Turret turret = hit.GetComponent<Turret>();
            if (turret == null)
                continue;

            turret.CleanseStatus();                   // EMP/해킹 상태 정화
            turret.ApplyShieldImmunity(buffDuration); // buffDuration 초간 면역 부여
            buffedTurrets.Add(turret);
        }

        // 버프 지속 시간 후 언차지 상태로 전환
        buffCoroutine = StartCoroutine(BuffDurationCoroutine());
    }

    private IEnumerator BuffDurationCoroutine()
    {
        float timer = 0f;
        while (timer < buffDuration)
        {
            // 보호막 포탑이 파괴된 경우 바로 종료
            if (currentDurability <= 0f)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        // 버프가 끝나면 언차지 상태로 전환
        buffedTurrets.Clear();
        currentState = ShieldState.Uncharged;
        waitingForRecharge = true;
        SetStateVisual(currentState);
    }

    /// <summary>
    /// 플레이어가 스페이스바를 눌러 호출 (InteractionManager를 통해)
    /// 언차지 상태일 때만 실행: Charged 상태 복귀
    /// </summary>
    public override void Interact()
    {
        if (!waitingForRecharge || currentDurability <= 0f) return;

        currentState = ShieldState.Charged;
        waitingForRecharge = false;
        SetStateVisual(currentState);
    }

    /// <summary>
    /// 상호작용 힌트 텍스트 (InteractionManager가 화면에 표시)
    /// 언차지 상태일 때만 반환
    /// </summary>
    public override string GetHintText()
    {
        if (currentState == ShieldState.Uncharged && currentDurability > 0f && playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist <= interactRange)
                return rechargeHint;
        }
        return string.Empty;
    }

    /// <summary>
    /// 시각적으로 상태에 따라 머테리얼 색상 변경
    /// </summary>
    private void SetStateVisual(ShieldState state)
    {
        Color col = unchargedColor;
        if (state == ShieldState.Charged) col = chargedColor;
        else if (state == ShieldState.Buffing) col = buffingColor;

        foreach (var rend in shieldRenderers)
        {
            rend.material.color = col;
        }
    }

    /// <summary>
    /// 외부에서 이 포탑에 데미지를 입혔을 때 호출됨
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (currentDurability <= 0f) return;

        currentDurability -= amount;
        if (currentDurability <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 파괴 처리: 버프 코루틴 정리 + InteractionManager 해제 + 오브젝트 파괴
    /// </summary>
    private void Die()
    {
        NotificationManager.Instance?.ShowSimple("쉴드포탑 파괴!", shieldTurretIcon);

        // 현재 보호막 포탑이 버프 중이었다면, 즉시 버프를 종료
        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        // 버프 중이라면, buffedTurrets 리스트를 순회하여 남아 있는 터렛들의 면역도 즉시 해제
        foreach (var turret in buffedTurrets)
        {
            turret.RemoveShieldImmunity();
        }
        buffedTurrets.Clear();

        // InteractionManager에서 해제
        InteractionManager.instance.Unregister(this);

        // 실제 오브젝트 파괴
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 혹시 오브젝트가 Disable-Enable 없이 파괴될 때 대비
        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        buffedTurrets.Clear();
    }

    // 디버그용: 반경 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shieldRadius);

        // 상호작용 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}

