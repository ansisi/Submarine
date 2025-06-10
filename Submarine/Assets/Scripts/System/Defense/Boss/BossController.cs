using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(FactionHandler))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("패턴 설정")]
    [SerializeField] private float patternInterval = 5f;          // 다음 패턴까지 대기 시간입니다.
    [SerializeField] private List<GameObject> monsterPrefabs;     // 소환할 몬스터 프리팹 리스트입니다.
    [SerializeField] private int spawnCount = 6;                  // 한 번에 소환할 몬스터 수입니다.
    [SerializeField] private float spawnRadius = 10f;             // 보스 주변 소환 반경입니다.

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
            // , OtherPattern1, OtherPattern2 등 이후 패턴 추가
        };
    }

    private IEnumerator PatternRunner()
    {
        while (currentHealth > 0)
        {
            if (!isPatternRunning)
            {
                isPatternRunning = true;

                // 랜덤 인덱스로 패턴 선택
                int idx = UnityEngine.Random.Range(0, patterns.Count);
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
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0f;  // 평면상 소환을 위해 높이 고정
            Vector3 spawnPos = transform.position + randomOffset;

            Instantiate(monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)],
                        spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);  // 소환 간격 (조정 가능)
        }
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
        // 보스 위치 기준으로 spawnRadius 반경을 시각화 (녹색 원)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
