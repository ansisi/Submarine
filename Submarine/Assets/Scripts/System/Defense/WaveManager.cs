using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int maxWaveCount = 10;                 // 최대 웨이브 수
    public float waveDuration = 180f;              // 한 웨이브당 진행 시간 (초)
    public float downtimeDuration = 60f;           // 웨이브 종료 후 정비 시간 (초)
    public float firstWaveDelayAfterTrigger = 15f; // 첫 웨이브 트리거 후 대기 시간 (초)

    public EnemySpawner enemySpawner;              // 적 스포너 참조

    [Header("웨이브 데이터")]
    public List<WaveData> waveDatas;                // 웨이브별 데이터

    private int currentWave = 0;
    private bool firstWaveTriggered = false;

    void Update()
    {
        if (!firstWaveTriggered && Input.GetKeyDown(KeyCode.T)) // 키를 누르면 첫 웨이브 트리거
        {
            firstWaveTriggered = true;
            StartCoroutine(FirstWaveRoutine());
        }
    }

    private IEnumerator FirstWaveRoutine()
    {
        yield return new WaitForSeconds(firstWaveDelayAfterTrigger);
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave < maxWaveCount)
        {
            currentWave++;
            Logger.Log($"Wave {currentWave} 시작!");

            if (currentWave - 1 < waveDatas.Count)
            {
                WaveData waveData = waveDatas[currentWave - 1];
                //웨이브 시작 시 BGM 전환
                BgmManager.Instance.StartCombat(); // 전투 BGM으로 변경
                yield return StartCoroutine(HandleWave(waveData));
            }

            Logger.Log($"Wave {currentWave} 종료. 정비 시간 시작!");
            // 정비 시간 동안 BGM 전환
            BgmManager.Instance.StartRepair(); // 정비 BGM으로 변경
            yield return new WaitForSeconds(downtimeDuration);
        }

        OnAllWavesCompleted();
    }

    private IEnumerator HandleWave(WaveData waveData)
    {
        // 서브 웨이브 개수
        int subWaveCount = waveData.subWaves.Count;

        // 서브 웨이브의 간격 계산 (웨이브 시간 / 서브 웨이브 개수)
        float subWaveInterval = waveDuration / subWaveCount;

        foreach (var subWave in waveData.subWaves)
        {
            enemySpawner.SpawnSubWave(subWave);

            // 서브 웨이브가 끝난 후, 주기만큼 기다리기
            yield return new WaitForSeconds(subWaveInterval);
        }
    }

    private void OnAllWavesCompleted()
    {
        Logger.Log("모든 웨이브 완료! 게임 승리!");
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}

[System.Serializable]
public class WaveData
{
    [Header("서브 웨이브 데이터")]
    public List<SubWaveData> subWaves; // 소웨이브 리스트
}

[System.Serializable]
public class SubWaveData
{
    [Header("적 스폰 데이터(적, 스폰 개수)")]
    public EnemySpawnData[] enemySpawnDatas; // 이 소웨이브에서 스폰할 적 종류와 수량
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public int spawnCount;          // 몇 마리 스폰할지
}

