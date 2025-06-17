using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Wave/Wave Data")]
public class WaveDataSO : ScriptableObject
{
    [Header("웨이브 지속 시간")]
    public float waveDuration = 180f;

    [Header("서브 웨이브 리스트")]
    public List<SubWaveData> subWaves;

    [Header("웨이브 클리어 보상")]
    public List<Item> clearRewardItems;    // 웨이브 클리어 시 지급할 아이템
    public List<int> clearRewardQuantities; // 지급할 아이템 수량
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
