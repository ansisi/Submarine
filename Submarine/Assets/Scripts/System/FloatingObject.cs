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
        rb.drag = 1f;                 // 느린 감속 효과
        rb.angularDrag = 0.05f;         // 회전 감속
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection.Normalize();

            // 질량에 따라 충돌 반작용 구현
            float forceMagnitude = objectMass / otherRb.mass;
            rb.AddForce(-forceDirection * forceMagnitude, ForceMode.Impulse);
            otherRb.AddForce(forceDirection * (0.5f / forceMagnitude), ForceMode.Impulse);
        }
    }
}
