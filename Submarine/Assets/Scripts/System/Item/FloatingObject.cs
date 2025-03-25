using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float objectMass = 1f; // 질량

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;          // 중력 제거
        rb.mass = objectMass;           // 질량 설정
    }

        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody otherRb = collision.rigidbody;
        // rb가 null인지 확인
        if (rb != null && otherRb != null)
        {
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection.Normalize();

            // 질량에 따라 충돌 반작용 구현
            float forceMagnitude = objectMass / otherRb.mass;
            rb.AddForce(-forceDirection * forceMagnitude, ForceMode.Impulse);  // rb가 null이 아닐 경우
            otherRb.AddForce(forceDirection * (0.5f / forceMagnitude), ForceMode.Impulse);
        }
        else
        {
            // 예외 처리: rb 또는 otherRb가 null인 경우 로그 출력
            if (rb == null)
            {
                Logger.LogWarning("Rigidbody가 할당되지 않았습니다. 충돌 반작용이 적용되지 않습니다.");
            }

            if (otherRb == null)
            {
                Logger.LogWarning("충돌한 물체에 Rigidbody가 없습니다.");
            }
        }
    }
}
