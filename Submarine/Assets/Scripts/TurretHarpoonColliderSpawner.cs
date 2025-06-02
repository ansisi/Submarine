using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITurretDeployable
{
    void OnDeployed();
}
/// <summary>
/// 포탑 전용 작살 상호작용용 콜라이더 생성기.
/// 포탑이 소환되면 자동으로 타원 모양으로 콜라이더를 생성하며,
/// 포탑 파괴 시 콜라이더도 같이 제거됨.
/// </summary>
public class TurretHarpoonColliderSpawner : MonoBehaviour, ITurretDeployable
{
    public int colliderCount = 20;            // 생성할 콜라이더 수
    public float radiusX = 2f;                // X축 반지름
    public float radiusY = 1f;                // Y축 반지름
    public Vector3 centerOffset = Vector3.zero; // 포탑 기준 중심 오프셋
    public float colliderRadius = 0.5f;       // 스피어 콜라이더 반지름

    public void OnDeployed()
    {
        SpawnColliders();
    }

    private void SpawnColliders()
    {
        Vector3 radiusCenter = transform.position + centerOffset;

        for (int i = 0; i < colliderCount; i++)
        {
            float angle = (360f / colliderCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 localOffset = new Vector3(Mathf.Cos(rad) * radiusX, Mathf.Sin(rad) * radiusY, 0f);

            GameObject colliderObj = new GameObject($"TurretCollider_{i}");
            colliderObj.transform.SetParent(transform, false);  // 부모 기준 로컬 좌표
            colliderObj.transform.localPosition = localOffset;

            colliderObj.tag = "Terrain";
            colliderObj.layer = LayerMask.NameToLayer("HarpoonOnly");

            SphereCollider sc = colliderObj.AddComponent<SphereCollider>();
            sc.radius = colliderRadius;
            sc.isTrigger = true; // 필요에 따라 트리거로 사용
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        int segments = 36;
        float angleStep = 360f / segments;

        Vector3 radiusCenter = transform.position + centerOffset;
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
