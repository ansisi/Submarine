using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(FactionHandler))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("패턴 설정")]
    [SerializeField] private float patternInterval = 5f;          // 다음 패턴까지 대기 시간입니다.

    [Header("몬스터 소환 설정")]  
    [SerializeField] private List<GameObject> monsterPrefabs;     // 소환할 몬스터 프리팹 리스트입니다.
    [SerializeField] private int spawnCount = 6;                  // 한 번에 소환할 몬스터 수입니다.
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // 소환 영역 사이즈 (X, Z)

    [Header("해킹 패턴 설정")]
    [SerializeField] private List<Vector3> hackZoneCenters; // 해킹 대상 구역들의 중심 위치 리스트
    [SerializeField] private Vector2 hackZoneSize = new Vector2(15f, 15f); // 해킹 박스 크기 (X, Z)
    [SerializeField] private GameObject hackIndicatorPrefab; // 인디케이터 프리팹 (반투명 빨간 박스)

    private List<Pattern> patterns;                      // 실행 가능한 패턴들의 델리게이트 리스트입니다.
    private bool isPatternRunning = false;                        // 패턴 실행 중복 방지 플래그입니다.
    private float maxHealth = 100f;                               // 보스 최대 체력입니다.
    private float currentHealth;                                  // 보스 현재 체력입니다.

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
        InitializePatterns();                                     // 패턴 리스트 초기화
        StartCoroutine(PatternRunner());                          // 패턴 실행 코루틴 시작
    }

    private void InitializePatterns()
    {
        patterns = new List<Pattern>()
        {
            new Pattern(SpawnMonsterPattern, 2f),  // 몬스터 소환 패턴, 시작 전 2초 대기
            new Pattern(HackTurretPattern, 5f),  // 해킹 패턴, 시작 전 3초 대기
            // , OtherPattern1, OtherPattern2 등 이후 패턴 추가
        };
    }

    private IEnumerator PatternRunner()
    {
        int lastPatternIndex = -1;  // 마지막 패턴 인덱스를 저장

        while (currentHealth > 0)
        {
            if (!isPatternRunning)
            {
                isPatternRunning = true;

                int idx;
                do
                {
                    idx = UnityEngine.Random.Range(0, patterns.Count);
                } while (idx == lastPatternIndex && patterns.Count > 1);

                lastPatternIndex = idx;
                Pattern selectedPattern = patterns[idx];

                // 패턴 시작 전 경고 및 대기
                yield return StartCoroutine(PatternWarning(selectedPattern.WarningTime, selectedPattern.PatternRoutine.Method.Name));

                // 패턴 실행
                yield return StartCoroutine(selectedPattern.PatternRoutine());

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

        // 2) 실제 해킹 실행
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

    public void TakeDamage(float amount)  // IDamageable 구현
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 보스 사망 처리 (폭발 이펙트, 보상 드랍 등)
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
    }
}
