using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    public GameObject[] traps; // 함정 프리팹 배열
    public int periodicTrapCount = 5; // 주기적으로 스폰될 함정 개수
    public Vector2 spawnAreaMin, spawnAreaMax; // 스폰 영역 (1구역)
    public float spawnInterval = 60f; // 스폰 주기

    private List<GameObject> spawnedTraps = new List<GameObject>(); // 현재 스폰된 함정 리스트

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnTraps(periodicTrapCount, spawnAreaMin, spawnAreaMax);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnTraps(int trapCount, Vector2 minArea, Vector2 maxArea)
    {
        for (int i = 0; i < trapCount; i++)
        {
            GameObject randomTrap = traps[Random.Range(0, traps.Length)];
            Vector3 spawnPosition = GetRandomPosition(minArea, maxArea);
            GameObject spawnedTrap = Instantiate(randomTrap, spawnPosition, Quaternion.identity);
            spawnedTraps.Add(spawnedTrap);
        }
    }

    Vector3 GetRandomPosition(Vector2 minArea, Vector2 maxArea)
    {
        float x = Random.Range(minArea.x, maxArea.x);
        float y = Random.Range(minArea.y, maxArea.y);
        return new Vector3(x, y, 0);
    }
}
