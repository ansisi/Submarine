using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] survivalItems; // 연료, 산소 아이템
    public GameObject[] resourceItems; // 부품 아이템
    public GameObject[] pipeModels;    // 파이프 모델 3종 (랜덤 스폰)

    public int initialSurvivalItemCount = 5; // 초기 생존 아이템 개수
    public int periodicSurvivalItemCount = 5; // 주기적 스폰 생존 아이템 개수
    public int initialResourceItemCount = 4;  // 초기 부품 아이템 개수
    public int periodicResourceItemCount = 4; // 주기적 스폰 부품 아이템 개수

    public Vector2 spawnAreaMin_1, spawnAreaMax_1; // 1구역 (초기 스폰 범위)
    public Vector2 spawnAreaMin_2, spawnAreaMax_2; // 2구역 (주기적 스폰 범위)

    public Submarine submarine; // 잠수함 참조

    public float spawnInterval = 60f;           // 아이템 스폰 주기 (초)
    public float survivalItemDecayRate = 0.9f;  // 생존 아이템 감소율 (12% 감소)
    public int minSurvivalItemCount = 2;        // 최소 생존 아이템 개수
    private int currentSurvivalItemCount;       //현재 생존 아이템 개수
    private bool isInitialSpawn = true;         // 처음 스폰 여부

    private List<GameObject> overlapSpheres = new List<GameObject>(); // 스폰 위치 확인용 오버랩 스피어 저장


    void Start()
    {

        currentSurvivalItemCount = periodicSurvivalItemCount;
        StartCoroutine(SpawnRoutine());
    }


    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (isInitialSpawn)
            {
                // 1구역에서 초기에 스폰
                SpawnItems(survivalItems, initialSurvivalItemCount, spawnAreaMin_1, spawnAreaMax_1);
                SpawnItems(resourceItems, initialResourceItemCount, spawnAreaMin_1, spawnAreaMax_1);
                SpawnPipes(initialResourceItemCount, spawnAreaMin_1, spawnAreaMax_1);
                isInitialSpawn = false;
            }
            else
            {
                // 2구역에서 주기적으로 스폰
                SpawnItems(survivalItems, currentSurvivalItemCount, spawnAreaMin_2, spawnAreaMax_2);
                SpawnItems(resourceItems, periodicResourceItemCount, spawnAreaMin_2, spawnAreaMax_2);
                SpawnPipes(periodicResourceItemCount, spawnAreaMin_2, spawnAreaMax_2);
            }

            yield return new WaitForSeconds(spawnInterval);

            // 생존 아이템 개수를 점진적으로 감소
            if (currentSurvivalItemCount > minSurvivalItemCount)
            {
                //RoundToInt : 가장 가까운 정수로 반올림
                currentSurvivalItemCount = Mathf.Max(minSurvivalItemCount, Mathf.RoundToInt(currentSurvivalItemCount * survivalItemDecayRate));
            }
        }
    }

    void SpawnItems(GameObject[] items, int itemCount, Vector2 minArea, Vector2 maxArea)
    {
        foreach (GameObject itemPrefab in items)
        {
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPosition = GetValidSpawnPosition(itemPrefab, minArea, maxArea);
                Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            }
        }

        ClearOverlapSpheres(overlapSpheres);
    }

    void SpawnPipes(int itemCount, Vector2 minArea, Vector2 maxArea)
    {
        for (int i = 0; i < itemCount; i++)
        {
            GameObject randomPipe = pipeModels[Random.Range(0, pipeModels.Length)]; // 랜덤 파이프 선택
            Vector3 spawnPosition = GetValidSpawnPosition(randomPipe, minArea, maxArea);
            Instantiate(randomPipe, spawnPosition, Quaternion.identity);
        }

        ClearOverlapSpheres(overlapSpheres); // 스폰이 끝나면 즉시 삭제

    }

    Vector3 GetValidSpawnPosition(GameObject itemPrefab,Vector2 minArea, Vector2 maxArea)
    {
        Vector3 position;
        float objectRadius = GetObjectRadius(itemPrefab); // 오브젝트 크기 기반 반지름 계산
        int maxAttempts = 20; // 무한 루프 방지
        int attempts = 0;

        do
        {
            float x = Random.Range(minArea.x, maxArea.x);
            float y = Random.Range(minArea.y, maxArea.y);
            position = new Vector3(x, y, 0);
            attempts++;
        }
        while ((IsOverlappingSubmarine(position) || IsOverlappingOtherObjects(position, objectRadius)) && attempts < maxAttempts);

        CreateOverlapSphere(position, objectRadius); // 새로운 위치에 오버랩 스피어 생성
        return position;
    }

    float GetObjectRadius(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds.extents.magnitude + 0.2f; // 오브젝트 크기 + 0.2의 여유 공간 추가
        }
        return 0.5f; // 기본값 (오브젝트에 콜라이더가 없을 경우)
    }

    bool IsOverlappingSubmarine(Vector3 position)
    {
        float submarineRadius = 3f; // 잠수함과의 최소 거리
        return Vector3.Distance(position, submarine.transform.position) < submarineRadius;
    }

    bool IsOverlappingOtherObjects(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        return colliders.Length > 0;
    }

    void CreateOverlapSphere(Vector3 position, float radius)
    {
        GameObject sphere = new GameObject("OverlapSphere");
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

        SphereCollider sphereCollider = sphere.AddComponent<SphereCollider>();
        sphereCollider.radius = radius;
        sphereCollider.isTrigger = true;

        overlapSpheres.Add(sphere);
    }

    void ClearOverlapSpheres(List<GameObject> overlapSpheres)
    {
        foreach (var sphere in overlapSpheres)
        {
            Destroy(sphere);
        }
        overlapSpheres.Clear();
    }

    // Gizmo로 스폰 영역을 그리기
    void OnDrawGizmos()
    {
        // 스폰 영역을 나타내는 Wireframe 박스를 그립니다.
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((spawnAreaMin_1.x + spawnAreaMax_1.x) / 2, (spawnAreaMin_1.y + spawnAreaMax_1.y) / 2, 0),
                            new Vector3(spawnAreaMax_1.x - spawnAreaMin_1.x, spawnAreaMax_1.y - spawnAreaMin_1.y, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3((spawnAreaMin_2.x + spawnAreaMax_2.x) / 2, (spawnAreaMin_2.y + spawnAreaMax_2.y) / 2, 0),
                            new Vector3(spawnAreaMax_2.x - spawnAreaMin_2.x, spawnAreaMax_2.y - spawnAreaMin_2.y, 0));
    }
}
