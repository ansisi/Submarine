using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave/BossWaveData")]
public class BossWaveDataSO : WaveDataSO
{
    public GameObject bossPrefab;         // 보스 프리팹
    public Vector3 spawnPoint;          // 소환 위치
    public float bossDelayTime = 10f;     // 잡몹 끝나고 보스 등장까지 딜레이
}