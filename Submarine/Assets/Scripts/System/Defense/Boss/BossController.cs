using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net.NetworkInformation;

[RequireComponent(typeof(FactionHandler))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("보스 설정")]
    [SerializeField] private float maxHealth = 100f;                               // 보스 최대 체력입니다.
    [SerializeField] private float currentHealth;                                  // 보스 현재 체력입니다.
    [SerializeField] private float patternInterval = 5f;          // 다음 패턴까지 대기 시간입니다.

    [Header("몬스터 소환 설정")]  
    [SerializeField] private List<GameObject> monsterPrefabs;     // 소환할 몬스터 프리팹 리스트입니다.
    [SerializeField] private int spawnCount = 6;                  // 한 번에 소환할 몬스터 수입니다.
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // 소환 영역 사이즈 (X, Z)

    [Header("해킹 패턴 설정")]
    [SerializeField] private List<Vector3> hackZoneCenters; // 해킹 대상 구역들의 중심 위치 리스트
    [SerializeField] private Vector2 hackZoneSize = new Vector2(15f, 15f); // 해킹 박스 크기 (X, Z)
    [SerializeField] private GameObject hackIndicatorPrefab; // 인디케이터 프리팹 (반투명 빨간 박스)

    [Header("EMP 패턴 설정")]
    [SerializeField] private float empCooldown = 20f;                 // EMP 패턴 쿨타임입니다.
    [SerializeField] private float empRadius = 10f;                   // EMP 효과 반경입니다.
    [SerializeField] private float empEffectDuration = 5f;            // 터렛에게 적용될 EMP 지속 시간입니다.
    [SerializeField] private GameObject empWarningPrefab;             // EMP 경고용 깜빡임 오브젝트
    [SerializeField] private float empWarningDuration = 2f;           // 경고 시간
    [SerializeField] private float empBlinkInterval = 0.2f;           // 깜빡이는 간격
    private float lastEmpTime = -Mathf.Infinity;                      // 마지막 EMP 실행 시간입니다.


    [Header("푸시 패턴 설정")]
    [SerializeField] private GameObject pushWavePrefab;                 // 푸시 경고용 파동(LineRenderer 등) 프리팹
    [SerializeField] private float pushWarningDuration = 1f;            // 파동 경고 지속 시간입니다.
    [SerializeField] private LayerMask pushableLayerMask;               // 푸시 가능한 레이어 마스크 (터렛, 플레이어 등)
    [SerializeField] private LayerMask playerLayerMask;  // 플레이어 레이어만 지정
    private float nextPushThreshold;                                    // 다음 푸시 패턴 발동 체력 임계치
    private bool pushPending = false;

    [Header("자기력 패턴 설정")]
    [SerializeField] private float pullRadius = 20f;            // 끌어당길 반경
    [SerializeField] private float pullDuration = 5f;           // 끌어당기는 지속 시간 (고정 5초)
    [SerializeField] private float pullSpeed = 5f;              // 끌어당기는 속도
    [SerializeField] private float pullDamage = 200f;           // 중심 도달 시 입힐 대미지
    [SerializeField] private float groggyDuration = 5f;         // 그로기 지속 시간
    [SerializeField] private float reachThreshold = 0.5f;       // 보스 중심 도달 판정 거리
    private float nextPullThreshold;     // 다음 Pull 패턴 발동 체력 기준
    private bool pullPending = false;    // Pull 패턴 대기 플래그
    private bool isGroggy = false;       // 그로기 상태 플래그

    [Header("자기력 패턴 이펙트 설정")]
    [SerializeField] private int pullEffectSegments = 64; // 원분할 수
    [SerializeField] private float pullEffectDuration = 1f; // 한 링이 줄어드는 시간
    [SerializeField] private float pullEffectInterval = 0.3f; // 링 생성 간격
    private List<GameObject> pullEffectInstances = new List<GameObject>();
    private Coroutine pullEffectCoroutine;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;



    private List<Pattern> patterns;                      // 실행 가능한 패턴들의 델리게이트 리스트입니다.
    private bool isPatternRunning = false;                        // 패턴 실행 중복 방지 플래그입니다.
    
    

    public Action<string> OnPatternWarningStarted;  // 경고 시작 시 호출 (패턴 이름 인자)
    public Action OnPatternWarningEnded;            // 경고 종료 시 호출
    // 패턴 정보를 묶는 클래스
    private class Pattern
    {
        public Func<IEnumerator> PatternRoutine;   // 패턴 실행 코루틴 함수
        public float WarningTime;                  // 패턴 시작 전 대기시간

        public Pattern(Func<IEnumerator> routine, float warningTime)
        {
            PatternRoutine = routine;
            WarningTime = warningTime;
        }
    }
    private void Start()
    {
        currentHealth = maxHealth;                                // 보스 체력 초기화

        BossUIManager.Instance.ShowFor(this);

        nextPushThreshold = maxHealth * 0.85f;                    // 15% 감소 시점마다 푸시
        nextPullThreshold = maxHealth * 0.7f;
        InitializePatterns();                                     // 패턴 리스트 초기화
        //StartCoroutine(PullPattern());
        StartCoroutine(PatternRunner());                   // 패턴 실행 코루틴 시작
    }

    private void InitializePatterns()
    {
        patterns = new List<Pattern>()
        {
            new Pattern(SpawnMonsterPattern, 3f),  // 몬스터 소환 패턴, 시작 전 2초 대기
            new Pattern(HackTurretPattern, 3f),  // 해킹 패턴, 시작 전 3초 대기
            new Pattern(EmpPattern, 3f), 
            // , OtherPattern1, OtherPattern2 등 이후 패턴 추가
        };
    }

    private IEnumerator PatternRunner()
    {
        while (currentHealth > 0)
        {
            if (!isPatternRunning && !isGroggy)
            {
                isPatternRunning = true;

                // PushBackPattern (가장 높은 우선순위) 
                if (pushPending)
                {
                    yield return StartCoroutine(PatternWarning(0f, nameof(PushBackPattern)));
                    yield return StartCoroutine(PushBackPattern());
                    pushPending = false;
                }
                // PullPattern
                else if (pullPending)
                {
                    yield return StartCoroutine(PatternWarning(2f, nameof(PullPattern)));
                    yield return StartCoroutine(PullPattern());
                    pullPending = false;
                }
                // EmpPattern (쿨다운 & 대상 있을 때)
                else if (Time.time - lastEmpTime >= empCooldown &&
                         Physics.OverlapSphere(transform.position, empRadius)
                             .Any(c => c.TryGetComponent<Turret>(out _)))
                {
                    yield return StartCoroutine(PatternWarning(3f, nameof(EmpPattern)));
                    yield return StartCoroutine(EmpPattern());
                }
                // SpawnMonsterPattern & HackTurretPattern (동일 우선순위)
                else
                {
                    // 랜덤으로 하나 선택
                    var list = new List<Pattern> 
                    {
                        new Pattern(SpawnMonsterPattern, 3f),
                        new Pattern(HackTurretPattern, 3f)
                    };
                    int idx = UnityEngine.Random.Range(0, list.Count);
                    var p = list[idx];
                    yield return StartCoroutine(PatternWarning(p.WarningTime, p.PatternRoutine.Method.Name));
                    yield return StartCoroutine(p.PatternRoutine());
                }

                yield return new WaitForSeconds(patternInterval);
                isPatternRunning = false;
            }
            yield return null;
        }
    }

    private IEnumerator PatternWarning(float warningTime, string patternName)
    {
        Logger.Log($"[패턴 경고] {patternName} 패턴이 {warningTime}초 후 시작됩니다.");
        OnPatternWarningStarted?.Invoke(patternName);  // 경고 시작 이벤트 호출

        yield return new WaitForSeconds(warningTime);

        OnPatternWarningEnded?.Invoke();               // 경고 종료 이벤트 호출
    }

    private IEnumerator SpawnMonsterPattern()
    {
        // 몬스터 6마리를 보스 주변에 랜덤 위치로 소환
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                UnityEngine.Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f), 0f); 
            Vector3 spawnPos = transform.position + offset;

            Instantiate(monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)],
                        spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);  // 소환 간격 (조정 가능)
        }
    }

    private IEnumerator HackTurretPattern()
    {
        // 가장 많은 터렛이 있는 구역 인덱스 찾기
        int maxTurretCount = -1;
        List<int> candidateZones = new List<int>();

        for (int i = 0; i < hackZoneCenters.Count; i++)
        {
            Vector3 center = hackZoneCenters[i];
            Vector3 halfExtents = new Vector3(hackZoneSize.x / 2f, hackZoneSize.y / 2f, 1f); // Z는 얇게
            
            Collider[] cols = Physics.OverlapBox(center, halfExtents);
            
            int count = cols.Count(c => c.TryGetComponent<Turret>(out _));

            if (count > maxTurretCount)
            {
                maxTurretCount = count;
                candidateZones.Clear();
                candidateZones.Add(i);
            }
            else if (count == maxTurretCount)
            {
                candidateZones.Add(i);
            }
        }

        // 터렛 수가 같은 구역이 여러 개면 그 중 랜덤 선택
        int bestZoneIndex = candidateZones[UnityEngine.Random.Range(0, candidateZones.Count)];

        // 인디케이터 프리팹 생성 (경고 시각화)
        GameObject indicator = Instantiate(hackIndicatorPrefab, hackZoneCenters[bestZoneIndex], Quaternion.identity);
        indicator.transform.localScale = new Vector3(hackZoneSize.x, hackZoneSize.y, 0.5f); // 얇은 Z

        // 3초간 깜빡임 효과 (PatternWarning 시간만큼 깜빡임 지속)
        float blinkDuration = 5f;
        float timer = 0f;
        float blinkInterval = 0.5f;
        bool visible = true;

        while (timer < blinkDuration)
        {
            indicator.SetActive(visible);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        Destroy(indicator); // 인디케이터 제거

        // 실제 해킹 실행
        Vector3 bestCenter = hackZoneCenters[bestZoneIndex];
        Vector3 bestHalfExtents = new Vector3(hackZoneSize.x / 2f, hackZoneSize.y / 2f, 1f);
        Collider[] targets = Physics.OverlapBox(bestCenter, bestHalfExtents);

        foreach (var col in targets)
        {
            if (col.TryGetComponent<Turret>(out var turret))
            {
                turret.SetHacked(true);
            }
        }

        yield break;
    }

    private IEnumerator EmpPattern()
    {
        // 쿨타임 확인
        if (Time.time - lastEmpTime < empCooldown)
        {
            Logger.Log("[EMP] 쿨타임이 아직 남아 있어 패턴을 취소합니다.");
            yield break;
        }

        // 반경 내 터렛 탐색
        Collider[] cols = Physics.OverlapSphere(transform.position, empRadius);
        var targets = cols
            .Where(c => c.TryGetComponent<Turret>(out _))
            .Select(c => c.GetComponent<Turret>())
            .ToList();

        if (targets.Count == 0)
        {
            Logger.Log("[EMP] 반경 내에 터렛이 없어 패턴을 취소합니다.");
            yield break;
        }

        // EMP 경고 표시
        yield return StartCoroutine(ShowEmpWarning());

        // 폭탄(Mine) 파괴 처리
        Collider[] mines = Physics.OverlapSphere(transform.position, empRadius);
        foreach (var col in mines)
        {
            if (col.TryGetComponent<Mine>(out var mine))
            {
                Destroy(mine.gameObject);  // 범위 내 모든 지뢰 파괴
            }
        }

        // 패턴 실행
        Logger.Log($"[EMP] {targets.Count}개의 터렛에 EMP 효과를 적용합니다.");
        foreach (var turret in targets)
        {
            turret.ApplyEMPEffect(empEffectDuration);  // EMP 효과 적용
        }

        lastEmpTime = Time.time;
        yield return null;
    }

    private IEnumerator PushBackPattern()
    {
        // 푸시 패턴 실행 전 경고 표시
        yield return StartCoroutine(ShowPushWarning());

        Logger.Log("[PushBack] 범위 내 터렛과 플레이어를 밀어냅니다.");

        int combinedMask = pushableLayerMask | (1 << LayerMask.NameToLayer("Player"));
        Collider[] cols = Physics.OverlapSphere(transform.position, empRadius, combinedMask);

        foreach (var col in cols)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 origin = transform.position;
                Vector3 targetPos = col.transform.position;

                // XY 평면 방향 벡터 계산
                Vector3 direction = targetPos - origin;
                direction.z = 0f;
                float distance = direction.magnitude;

                if (distance < empRadius)
                {
                    direction.Normalize();

                    // 목표 위치 계산 (EMP 범위 경계선 위치)
                    Vector3 destination = origin + direction * empRadius;

                    // 코루틴으로 밀어내기 시작
                    StartCoroutine(PushToPosition(rb, destination, 0.15f));
                }
            }
        }

        yield return null;
    }

    private IEnumerator ShowPushWarning()
    {
        // 푸시 파동 프리팹에서 LineRenderer 가져오기
        GameObject wave = Instantiate(pushWavePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        LineRenderer lineRenderer = wave.GetComponent<LineRenderer>();  // LineRenderer 컴포넌트
        if (lineRenderer == null)
        {
            Debug.LogWarning("pushWavePrefab에 LineRenderer가 없습니다.");
            Destroy(wave);
            yield break;
        }

        // 파동 설정 변수
        int segments = 64;                                      // 원을 구성할 세그먼트 수
        float elapsed = 0f;
        float startRadius = 0f;
        float endRadius = empRadius;                           
        float duration = pushWarningDuration;                   // 지속 시간

        // LineRenderer 초기 설정
        lineRenderer.positionCount = segments + 1;                        // 마지막 점이 시작점과 같게
        lineRenderer.useWorldSpace = false;                               // 로컬 좌표계 사용

        // 경고 파동 애니메이션
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentRadius = Mathf.Lerp(startRadius, endRadius, t);

            // 원형 좌표 계산
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float x = Mathf.Cos(angle) * currentRadius;
                float z = Mathf.Sin(angle) * currentRadius;
                lineRenderer.SetPosition(i, new Vector3(x, 0f, z));      // y=0 평면에 그리기
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 완료 후 제거
        Destroy(wave);
    }

    private IEnumerator PushToPosition(Rigidbody rb, Vector3 destination, float duration)
    {
        float timer = 0f;
        Vector3 start = rb.position;

        // 기존 constraints 저장
        RigidbodyConstraints originalConstraints = rb.constraints;

        // X, Y 축만 해제 (Z는 그대로 유지)
        rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);

        while (timer < duration)
        {
            // z는 고정 (XY 평면으로만 이동)
            Vector3 nextPos = Vector3.Lerp(start, destination, timer / duration);
            nextPos.z = rb.position.z;

            rb.MovePosition(nextPos);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 위치 보정
        destination.z = rb.position.z;
        rb.MovePosition(destination);

        // 속도 정지
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // constraints 원래대로 복원
        rb.constraints = originalConstraints;
    }

    private IEnumerator PullPattern()
    {
        Logger.Log("[Pull] 범위 내 오브젝트를 끌어당깁니다.");

        Collider[] bossCols = GetComponentsInChildren<Collider>();
        foreach (var bc in bossCols)
        {
            bc.isTrigger = true;
        }

        // 끌어당김 이펙트 시작
        pullEffectCoroutine = StartCoroutine(ShowPullEffect());

        //초기 대상 수집
        List<Rigidbody> targets = new List<Rigidbody>();
        float elapsed = 0f;
        var originalConstraints = new Dictionary<Rigidbody, RigidbodyConstraints>();
        

        // 끌어당기는 코루틴
        while (elapsed < pullDuration)
        {
            // 매 프레임 새로 들어온 대상 추가
            Collider[] cols = Physics.OverlapSphere(transform.position, pullRadius, pushableLayerMask);
            foreach (var col in cols)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !targets.Contains(rb))
                {
                    targets.Add(rb);                                        // targets 리스트에 추가합니다.
                    originalConstraints[rb] = rb.constraints;               // 원래 제약도 저장합니다.
                }
            }

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                var rb = targets[i];
                if (rb == null)
                {
                    targets.RemoveAt(i); 
                    continue;
                }

                // 끌어당기기 시작 전, X/Y Freeze 해제
                rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);

                Vector3 dir = (transform.position - rb.position);
                dir.z = 0f;
                float dist = dir.magnitude;
                dir.Normalize();

                // 목표 위치(보스 중심 경계선) 계산
                Vector3 destination = transform.position;

                // 폭탄(Bomb) 감지
                if (dist <= reachThreshold && rb.TryGetComponent<Mine>(out var mine))
                {
                    yield return StartCoroutine(mine.ExplodeAfterDelay());

                    // 즉시 패턴 종료 → 그로기 상태 돌입
                    StartCoroutine(GroggyState());
                    // Pull 이펙트 중지
                    if (pullEffectCoroutine != null)
                        StopCoroutine(pullEffectCoroutine);

                    // 보스 콜라이더 복원 전에 내부에 갇힌 플레이어만 밀어냅니다.
                    Collider[] stuckPlayers = Physics.OverlapSphere(transform.position, reachThreshold, playerLayerMask);
                    foreach (var col in stuckPlayers)
                    {
                        Transform t = col.transform;
                        Vector3 dirt = (t.position - transform.position).normalized;    // 보스 중심에서 바깥 방향
                        Vector3 safePos = transform.position + dirt * (reachThreshold + 0.1f); // 기준 거리 + 여유
                        t.position = safePos;  // 플레이어 위치 강제 이동
                    }

                    // 남아있는 모든 풀 이펙트 인스턴스 파괴
                    foreach (var effect in pullEffectInstances)
                        Destroy(effect);
                    pullEffectInstances.Clear();

                    foreach (var bc in bossCols)
                        bc.isTrigger = false;


                    elapsed = pullDuration;    // 즉시 종료

                    // 모든 constraints 원복
                    foreach (var kv in originalConstraints)
                        if (kv.Key != null)
                            kv.Key.constraints = kv.Value;

                    yield break;
                }

                // 중심 도달 시 파괴
                if (dist <= reachThreshold)
                {
                    rb.GetComponent<IDamageable>()?.TakeDamage(pullDamage);
                    targets.RemoveAt(i);
                    continue;
                }

                // 이동
                Vector3 next = rb.position + dir * pullSpeed * Time.deltaTime;
                next.z = rb.position.z;
                rb.MovePosition(next);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Pull 이펙트 중지
        if (pullEffectCoroutine != null)
            StopCoroutine(pullEffectCoroutine);

        foreach (var bc in bossCols)
        {
            bc.isTrigger = false; 
        }

        // 남은 타겟들 전부 constraints 복원
        foreach (var kv in originalConstraints)
            if (kv.Key != null)
                kv.Key.constraints = kv.Value;
    }

    private IEnumerator ShowPullEffect()
    {
        float totalTime = 0f;
        while (totalTime < pullDuration)
        {
            // 매 간격마다 새로운 링 코루틴을 시작
            StartCoroutine(AnimatePullRing());

            yield return new WaitForSeconds(pullEffectInterval);
            totalTime += pullEffectInterval;
        }
    }

    private IEnumerator AnimatePullRing()
    {
        // 링 오브젝트 생성
        GameObject instantiate = Instantiate(pushWavePrefab, transform.position, Quaternion.identity);
        pullEffectInstances.Add(instantiate);   
        LineRenderer lineRenderer = instantiate.GetComponent<LineRenderer>();

        if (lineRenderer == null) yield break;

        lineRenderer.positionCount = pullEffectSegments + 1;
        lineRenderer.useWorldSpace = true;

        float elapsed = 0f;
        while (elapsed < pullEffectDuration)
        {
            if (lineRenderer == null) yield break;

            float effectDuration = elapsed / pullEffectDuration;
            float currentRadius = Mathf.Lerp(pullRadius, 0f, effectDuration);

            // 원형 좌표 계산
            for (int i = 0; i <= pullEffectSegments; i++)
            {
                float effectSegments = 2f * Mathf.PI * i / pullEffectSegments;
                Vector3 pos = transform.position + new Vector3(Mathf.Cos(effectSegments), Mathf.Sin(effectSegments), 0f) * currentRadius;
                lineRenderer.SetPosition(i, pos);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 프레임에서 완전 축소
        if (lineRenderer != null)
        {
            for (int i = 0; i <= pullEffectSegments; i++)
                lineRenderer.SetPosition(i, transform.position);
        }

        pullEffectInstances.Remove(instantiate);
        Destroy(instantiate);
    }

    private IEnumerator GroggyState()
    {
        isGroggy = true;
        // 원한다면 애니메이션/이펙트 추가
        yield return new WaitForSeconds(groggyDuration);
        isGroggy = false;
    }

    public void TakeDamage(float amount)  // IDamageable 구현
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }

        // 15% 감소마다 푸시 발동
        if (currentHealth <= nextPushThreshold)
        {
            pushPending = true;
            nextPushThreshold -= maxHealth * 0.15f;
        }

        // Pull 예약 (30% 단위)
        if (currentHealth <= nextPullThreshold)
        {
            pullPending = true;
            nextPullThreshold -= maxHealth * 0.3f;
        }
    }

    private IEnumerator ShowEmpWarning()
    {
        GameObject warning = Instantiate(empWarningPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        warning.transform.localScale = new Vector3(empRadius * 2, 0.05f, empRadius * 2); // 중심에서 반지름 * 2가 되도록

        Renderer renderer = warning.GetComponent<Renderer>();
        if (renderer == null)
        {
            Logger.LogWarning("경고 오브젝트에 Renderer가 없습니다.");
            yield break;
        }

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < empWarningDuration)
        {
            renderer.enabled = visible;
            visible = !visible;

            yield return new WaitForSeconds(empBlinkInterval);
            elapsed += empBlinkInterval;
        }

        Destroy(warning);
    }

    private void Die()
    {
        BossUIManager.Instance.Hide();

        AudioManager.Instance.StartRepair();

        if (pullEffectCoroutine != null)
            StopCoroutine(pullEffectCoroutine);

        foreach (var effect in pullEffectInstances)
            Destroy(effect);
        pullEffectInstances.Clear();

        // 보스 사망 처리 (폭발 이펙트, 보상 드랍 등)
        CameraController.Instance.ExitBossCameraMode(); // 보스 카메라 모드 종료
        BossArenaManager.Instance.DisableArena(); // 보스전 아레나 비활성화
        GameOverUIManager.Instance.ShowGameClearUI();
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        // 소환 영역과 해킹 영역을 Gizmos로 시각화
        Gizmos.color = Color.green;
        Vector3 spawnSize = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.5f); // 얇은 Z
        Gizmos.DrawWireCube(transform.position, spawnSize);

        // 해킹 영역들 시각화
        Gizmos.color = Color.red;
        foreach (Vector3 center in hackZoneCenters)
        {
            Vector3 hackSize = new Vector3(hackZoneSize.x, hackZoneSize.y, 0.5f);
            Gizmos.DrawWireCube(center, hackSize);
        }

        // EMP 반경 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, empRadius);  // EMP 반경 시각화

        //풀 패턴 반경 (pullRadius)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        // 보스 중심 도달 판정 거리 (reachThreshold)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, reachThreshold);
    }
}
