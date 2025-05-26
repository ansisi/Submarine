using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreObject : MonoBehaviour
{
    private OreSpawner spawner;
    private int spawnIndex;

    public void Initialize(OreSpawner oreSpawner, int index)
    {
        spawner = oreSpawner;
        spawnIndex = index;
    }

    // 이 함수는 플레이어가 광석을 캤을 때 호출되도록 연결
    public void OnMined()
    {
        spawner.HandleOreMined(spawnIndex);
        Destroy(gameObject);
    }
}
