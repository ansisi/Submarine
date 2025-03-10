using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] survivalItems; // ����, ��� ������
    public GameObject[] resourceItems; // ��ǰ ������

    public int survivalItemCount = 10; // ����, ��� ������ ����
    public int resourceItemCount = 7; // �� ��ǰ ������ ����

    public Vector2 spawnAreaMin; // ���� ������ �ּ� ��ǥ (x, y)
    public Vector2 spawnAreaMax; // ���� ������ �ִ� ��ǥ (x, y)

    public Submarine submarine; // ����� ����

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
        float submarineRadius = 3f; // ����԰��� �ּ� �Ÿ�
        return Vector3.Distance(position, submarine.transform.position) < submarineRadius;
    }
}
