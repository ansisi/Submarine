using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private int aliveEnemyCount = 0; // 현재 웨이브의 살아있는 적 수

    [Header("웨이브 설정")]
    public float preparationTime = 30f; // 웨이브 전 준비 시간
    public List<WaveDataSO> waveList;

    public EnemySpawner enemySpawner;

    private int currentWave = 0;
    private bool isWaveRunning = false;
    private bool allSubWavesSpawned;

    public event Action<int> OnWaveStarted;   // 웨이브 시작 이벤트 (UI 연결용 등)
    public event Action<int> OnWaveEnded;     // 웨이브 종료 이벤트 (UI, BGM 등)

    private Coroutine waveRoutineCoroutine;  // WaveRoutine 코루틴 핸들 저장

    public event Action OnPreparationStarted;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            TriggerWaveStart();
        }
    }

    // 외부에서 호출될 트리거 함수
    public void TriggerWaveStart()
    {
        if (isWaveRunning || currentWave >= waveList.Count)
            return;

        OnPreparationStarted?.Invoke();

        StartCoroutine(WavePreparationRoutine());
    }

    private IEnumerator WavePreparationRoutine()
    {
        Logger.Log($"Wave {currentWave + 1} 준비 시작 (준비 시간 {preparationTime}초)");
        yield return new WaitForSeconds(preparationTime);
        waveRoutineCoroutine = StartCoroutine(WaveRoutine(waveList[currentWave]));
    }

    private IEnumerator WaveRoutine(WaveDataSO waveData)
    {
        isWaveRunning = true;
        allSubWavesSpawned = false;
        aliveEnemyCount = 0;  // 웨이브 시작 시 적 수 초기화

        OnWaveStarted?.Invoke(currentWave);
        Logger.Log($"Wave {currentWave + 1} 시작!");

        AudioManager.Instance.StartCombat();

        float waveStartTime = Time.time;

        var subWaves = new List<SubWaveData>(waveData.subWaves);
        subWaves.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));

        foreach (var subWave in subWaves)
        {
            if (subWave.spawnTime > waveData.waveDuration - 5f)
            {
                Logger.Log($"[서브웨이브 스킵됨] {subWave.spawnTime}초 (마지막 5초 안)");
                continue;
            }

            float wait = waveStartTime + subWave.spawnTime - Time.time;
            if (wait > 0)
                yield return new WaitForSeconds(wait);

            enemySpawner.SpawnSubWave(subWave);
        }
        allSubWavesSpawned = true;

        float remaining = waveStartTime + waveData.waveDuration - Time.time;
        if (remaining > 0)
            yield return new WaitForSeconds(remaining);

        while (aliveEnemyCount > 0)
            yield return null;

        EndCurrentWave();
    }

    public void RegisterSpawnedEnemies(int count)
    {
        aliveEnemyCount += count; // 스폰될 때 호출
    }

    public void OnEnemyKilled()
    {
        aliveEnemyCount--; // 적이 죽을 때마다 호출

        
        if (isWaveRunning && allSubWavesSpawned && aliveEnemyCount <= 0)
        {
            Logger.Log($"Wave {currentWave + 1} 모든 적 처치 완료!");

            if (waveRoutineCoroutine != null)
            {
                StopCoroutine(waveRoutineCoroutine); // 웨이브 진행 코루틴만 중지
                waveRoutineCoroutine = null;
            }
            EndCurrentWave();
        }
    }

    private void EndCurrentWave()
    {
        Logger.Log($"Wave {currentWave + 1} 종료! (보상 지급)");

        OnWaveEnded?.Invoke(currentWave);
        AudioManager.Instance.StartRepair();

        var waveData = waveList[currentWave];

        GiveClearReward(waveList[currentWave]);

        isWaveRunning = false;
        currentWave++;


        if (waveData is BossWaveDataSO bossWaveData)
        {
            StartCoroutine(StartBossPhaseRoutine(bossWaveData));
            return;
        }
    }

    private IEnumerator StartBossPhaseRoutine(BossWaveDataSO bossWaveData)
    {
        Logger.Log($"보스 소환까지 {bossWaveData.bossDelayTime}초 대기");
        yield return new WaitForSeconds(bossWaveData.bossDelayTime);

        GameObject boss = Instantiate(bossWaveData.bossPrefab, bossWaveData.spawnPoint, Quaternion.identity);
        BossArenaManager.Instance.EnableArena(); // 보스전 아레나 활성화
        CameraController.Instance.EnterBossCameraMode(); // 보스 카메라 모드로 전환
        Logger.Log("보스 2페이즈 시작!");

        // 여기에 보스 패턴 초기화, UI 변경 등 추가
    }

    private void GiveClearReward(WaveDataSO waveData)
    {
        // clearRewardItems와 clearRewardQuantities 리스트를 순회하며 보상 지급
        for (int i = 0; i < waveData.clearRewardItems.Count; i++)
        {
            var item = waveData.clearRewardItems[i];                // 보상 아이템
            int quantity = waveData.clearRewardQuantities[i];            // 해당 아이템 수량

            if (item != null && quantity > 0)
            {
                InventoryManager.Instance.AddItem(item, quantity);
                Logger.Log($"보상 지급: {item.name} x{quantity}");
            }
        }
    }

    public int GetCurrentWave() => currentWave;
    public bool IsWaveRunning() => isWaveRunning;
}



