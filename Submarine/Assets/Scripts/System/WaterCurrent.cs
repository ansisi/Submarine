using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
    public float currentStrength = 5f; // 물살의 강도
    public float currentRange = 3f;   // 물살의 범위
    public float currentAngle = 0f;   // 물살의 방향 (0~360도, XY 평면 기준)

    private Vector3 currentDirection; // 실제 적용할 물살 방향
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true; // Trigger로 설정
        }
        UpdateCurrentDirection();
        UpdateColliderSize();
    }

    void UpdateCurrentDirection()
    {
        // XY 평면에서의 방향 벡터로 변환
        float radian = currentAngle * Mathf.Deg2Rad;
        currentDirection = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f).normalized;
    }

    // 물살 범위에 따라 Collider 크기 업데이트
    void UpdateColliderSize()
    {
        if (boxCollider != null)
        {
            boxCollider.size = new Vector3(currentRange, currentRange, 1f); // Z축은 얇게 유지
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // 질량에 반비례한 힘 적용 (질량이 클수록 덜 밀림)
            float forceMultiplier = 1f / Mathf.Max(rb.mass, 0.1f);
            rb.AddForce(currentDirection * (currentStrength * 0.1f) * forceMultiplier, ForceMode.Acceleration);
        }
    }

    private void OnValidate()
    {
        UpdateCurrentDirection();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(currentRange, currentRange, 0f));

        // 물살 방향 화살표 표시
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = arrowStart + currentDirection * 2f;
        Gizmos.DrawLine(arrowStart, arrowEnd);

        Vector3 right = Quaternion.Euler(0, 0, 30) * -currentDirection;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -currentDirection;
        Gizmos.DrawLine(arrowEnd, arrowEnd + right * 0.5f);
        Gizmos.DrawLine(arrowEnd, arrowEnd + left * 0.5f);
    }
}
