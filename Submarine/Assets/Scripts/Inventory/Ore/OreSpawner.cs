using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    public GameObject orePrefab;       // 다양한 광석 프리팹 배열
    public int oreCount = 20;          // 생성할 광석 수
    public float respawnDelay = 5f; // 재생성 대기 시간
    private List<OreSpawnPoint> spawnPoints = new List<OreSpawnPoint>();
    public float radiusX = 5f;         // X축 반지름 (가로)
    public float radiusY = 3f;         // Y축 반지름 (세로)
    public Vector3 radiusCenter;       // 범위의 중심

    private void Start()
    {
        SpawnOres();
    }

    private void SpawnOres()
    {
        for (int i = 0; i < oreCount; i++)
        {
            float angle = (360f / oreCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            // XY 평면상의 타원 방향 계산 (Z는 고정)
            Vector3 direction = new Vector3(Mathf.Cos(rad) * radiusX, Mathf.Sin(rad) * radiusY, 0f);

            // radiusCenter를 기준으로 광석의 생성 위치 계산
            Vector3 spawnPos = radiusCenter + direction;

            // 광석 앞면이 원 중심을 바라보도록 회전 설정
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -direction);

            GameObject ore = Instantiate(orePrefab, spawnPos, rotation);
            // 부모로 설정하되, 월드 좌표 기준으로 유지 → 스케일 영향 없음
            ore.transform.SetParent(transform, true);

            // OreSpawnPoint 리스트에 저장
            spawnPoints.Add(new OreSpawnPoint { position = spawnPos, rotation = rotation, isOccupied = true });

            // OreObject 스크립트에서 이 spawner에 접근할 수 있도록 연결
            OreObject oreObject = ore.GetComponent<OreObject>();
            if (oreObject != null)
            {
                oreObject.Initialize(this, i); // i는 spawnPoints 인덱스
            }
        }
    }

    public void HandleOreMined(int index)
    {
        if (index < 0 || index >= spawnPoints.Count)
            return;

        spawnPoints[index].isOccupied = false;
        StartCoroutine(RespawnOreAfterDelay(index));
    }

    private IEnumerator RespawnOreAfterDelay(int index)
    {
        yield return new WaitForSeconds(respawnDelay);

        var point = spawnPoints[index];
        GameObject ore = Instantiate(orePrefab, point.position, point.rotation);
        ore.transform.SetParent(transform, true);

        spawnPoints[index].isOccupied = true;

        OreObject oreObject = ore.GetComponent<OreObject>();
        if (oreObject != null)
        {
            oreObject.Initialize(this, index);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        int segments = 36;
        float angleStep = 360f / segments;

        Vector3 prevPoint = radiusCenter + new Vector3(radiusX, 0f, 0f);  // X축 반지름 적용
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 nextPoint = radiusCenter + new Vector3(Mathf.Cos(rad) * radiusX, Mathf.Sin(rad) * radiusY, 0f);  // X, Y 반지름 적용

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}

[System.Serializable]
public class OreSpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;
    public bool isOccupied; // 현재 광석이 살아있는지
}
