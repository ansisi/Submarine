using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("웨이브 설정")]
    public float preparationTime = 30f; // 웨이브 전 준비 시간
    public List<WaveDataSO> waveList;

    public EnemySpawner enemySpawner;

    private int currentWave = 0;
    private bool isWaveRunning = false;

    public event Action<int> OnWaveStarted;   // 웨이브 시작 이벤트 (UI 연결용 등)
    public event Action<int> OnWaveEnded;     // 웨이브 종료 이벤트 (UI, BGM 등)

    private void Start()
    {
        // 웨이브 트리거는 외부에서 호출
    }

    // 외부에서 호출될 트리거 함수
    public void TriggerWaveStart()
    {
        if (isWaveRunning || currentWave >= waveList.Count)
            return;

        StartCoroutine(WavePreparationRoutine());
    }

    private IEnumerator WavePreparationRoutine()
    {
        Logger.Log($"Wave {currentWave + 1} 준비 시작 (준비 시간 {preparationTime}초)");
        yield return new WaitForSeconds(preparationTime);
        StartCoroutine(WaveRoutine(waveList[currentWave]));
    }

    private IEnumerator WaveRoutine(WaveDataSO waveData)
    {
        isWaveRunning = true;

        OnWaveStarted?.Invoke(currentWave);
        Logger.Log($"Wave {currentWave + 1} 시작!");

        BgmManager.Instance.StartCombat();

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

        float remaining = waveStartTime + waveData.waveDuration - Time.time;
        if (remaining > 0)
            yield return new WaitForSeconds(remaining);

        Logger.Log($"Wave {currentWave + 1} 종료! (이제 파밍 시간)");

        OnWaveEnded?.Invoke(currentWave);
        BgmManager.Instance.StartRepair();

        isWaveRunning = false;
        currentWave++;

        // 다음 웨이브는 다음 트리거로 시작됨 (대기 상태)
    }

    public int GetCurrentWave() => currentWave;
    public bool IsWaveRunning() => isWaveRunning;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Wave/Wave Data")]
public class WaveDataSO : ScriptableObject
{
    [Header("웨이브 지속 시간")]
    public float waveDuration = 180f;

    [Header("서브 웨이브 리스트")]
    public List<SubWaveData> subWaves;
}

[System.Serializable]
public class SubWaveData
{
    [Header("웨이브 시작 후 스폰될 시간 (초)")]
    public float spawnTime; // 예: 30f이면 웨이브 시작 후 30초에 스폰

    [Header("적 스폰 데이터(적, 스폰 개수)")]
    public EnemySpawnData[] enemySpawnDatas; // 이 소웨이브에서 스폰할 적 종류와 수량
    
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public int spawnCount;          // 몇 마리 스폰할지
}

