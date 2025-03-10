using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] survivalItems; // 연료, 산소 아이템
    public GameObject[] resourceItems; // 부품 아이템

    public int survivalItemCount = 10; // 연료, 산소 아이템 수량
    public int resourceItemCount = 7; // 각 부품 아이템 수량

    public Vector2 spawnAreaMin; // 스폰 가능한 최소 좌표 (x, y)
    public Vector2 spawnAreaMax; // 스폰 가능한 최대 좌표 (x, y)

    public Submarine submarine; // 잠수함 참조

    void Start()
    {
        SpawnItems(survivalItems, survivalItemCount);
        SpawnItems(resourceItems, resourceItemCount);
    }

    void SpawnItems(GameObject[] items, int itemCount)
    {
        foreach (GameObject itemPrefab in items)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPosition = GetValidSpawnPosition();
                Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 position;
        do
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            position = new Vector3(x, y, 0);
        }
        while (IsOverlappingSubmarine(position));

        return position;
    }

    bool IsOverlappingSubmarine(Vector3 position)
    {
        float submarineRadius = 3f; // 잠수함과의 최소 거리
        return Vector3.Distance(position, submarine.transform.position) < submarineRadius;
    }
}
