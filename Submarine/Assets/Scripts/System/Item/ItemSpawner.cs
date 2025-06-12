using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("공통 설정")]
    public GameObject[] itemPrefabs; // 생성할 아이템들
    public int initialSpawnCount = 10;
    public LayerMask obstacleMask; // 아이템이 스폰되면 안 되는 레이어

    [Header("1차 범위 (정원형)")]
    public Transform firstSpawnCenter;
    public float firstSpawnRadius = 5f;

    [Header("2차 범위 (타원형 가능)")]
    public Transform secondSpawnCenter;
    public float secondSpawnRadiusX = 30f;
    public float secondSpawnRadiusY = 100f;
    public float spawnInterval = 3f;    // 3초 간격으로 아이템 생성

    [Header("충돌 검사 설정")]
    public float checkRadius = 0.5f; // 해당 반지름 안에 오브젝트가 있으면 스폰 안 함
    public int maxSpawnAttempts = 10; // 충돌 시 몇 번까지 위치 재시도할지

    [Header("아이템을 담을 부모 오브젝트")]
    public Transform itemContainer; // 생성된 아이템들의 부모 오브젝트

    void Start()
    {
        // 1차: 시작 시 여러 개 생성
        for (int i = 0; i < initialSpawnCount; i++)
        {
            TrySpawnItem(firstSpawnCenter.position, firstSpawnRadius, isEllipse: false);
        }

        // 2차: 3초 간격으로 하나씩 생성
        StartCoroutine(SpawnItemPeriodically());
    }

    void TrySpawnItem(Vector3 center, float radiusOrX, bool isEllipse = false)
    {
        if (itemPrefabs.Length == 0) return;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 spawnPos;

            if (!isEllipse)
            {
                // 1차: 정원형 범위 (X, Y 동일)
                Vector2 offset = Random.insideUnitCircle * radiusOrX;
                spawnPos = center + new Vector3(offset.x, offset.y, 0f);
            }
            else
            {
                // 2차: 타원형 범위 (X, Y 따로 조절 가능)
                Vector2 offset = Random.insideUnitCircle;
                spawnPos = center + new Vector3(offset.x * secondSpawnRadiusX, offset.y * secondSpawnRadiusY, 0f);
            }

            // 충돌 검사
            if (Physics.CheckSphere(spawnPos, checkRadius, obstacleMask)) continue;

            // 생성
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

            // 부모 오브젝트에 추가
            if (itemContainer != null)
            {
                item.transform.SetParent(itemContainer);
            }

            return;
        }
    }

    IEnumerator SpawnItemPeriodically()
    {
        while (true)
        {
            TrySpawnItem(secondSpawnCenter.position, 0f, isEllipse: true);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void ExpandSecondSpawnAreaByTwo()
    {
        // 2차 범위 (타원형) 스케일 2배
        secondSpawnRadiusX *= 1.5f;
        secondSpawnRadiusY *= 2f;

        // 중심 위치 X를 2배로 이동
        if (secondSpawnCenter != null)
        {
            Vector3 pos = secondSpawnCenter.position;
            secondSpawnCenter.position = new Vector3(pos.x * 2.1f, pos.y, pos.z);
        }
    }


    void OnDrawGizmosSelected()
    {
        // 첫 번째 범위(정원형)
        if (firstSpawnCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firstSpawnCenter.position, firstSpawnRadius);
        }

        // 두 번째 범위(타원형)
        if (secondSpawnCenter != null)
        {
            Gizmos.color = Color.cyan;

            // 타원 형태를 선으로 그리기 (36개 점을 연결)
            int segments = 36; // 타원의 점 개수
            Vector3 center = secondSpawnCenter.position;
            Vector3 prevPoint = center + new Vector3(secondSpawnRadiusX, 0f, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * secondSpawnRadiusX;
                float y = Mathf.Sin(angle) * secondSpawnRadiusY;
                Vector3 newPoint = center + new Vector3(x, y, 0f);

                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}