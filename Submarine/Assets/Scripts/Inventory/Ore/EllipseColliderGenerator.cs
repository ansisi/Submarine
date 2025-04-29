using UnityEngine;

public class EllipseColliderSpawner : MonoBehaviour
{
    public int colliderCount = 20;       // 생성할 콜라이더 수
    public float radiusX = 5f;            // X축 반지름
    public float radiusY = 3f;            // Y축 반지름
    public Vector3 radiusCenter;          // 중심 위치
    public float colliderRadius = 0.2f;   // 스피어 콜라이더 반지름

    private void Start()
    {
        SpawnColliders();
    }

    private void SpawnColliders()
    {
        for (int i = 0; i < colliderCount; i++)
        {
            float angle = (360f / colliderCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(rad) * radiusX, Mathf.Sin(rad) * radiusY, 0f);
            Vector3 spawnPos = radiusCenter + direction;

            GameObject colliderObj = new GameObject("Collider_" + i);
            colliderObj.transform.SetParent(transform, true);
            colliderObj.transform.position = spawnPos;

            colliderObj.tag = "Terrain";
            colliderObj.layer = LayerMask.NameToLayer("HarpoonOnly");

            SphereCollider sc = colliderObj.AddComponent<SphereCollider>();
            sc.radius = colliderRadius;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        int segments = 36;
        float angleStep = 360f / segments;

        Vector3 prevPoint = radiusCenter + new Vector3(radiusX, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 nextPoint = radiusCenter + new Vector3(Mathf.Cos(rad) * radiusX, Mathf.Sin(rad) * radiusY, 0f);

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}