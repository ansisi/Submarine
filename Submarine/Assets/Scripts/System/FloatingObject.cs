using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float objectMass = 1f; // ����

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;          // �߷� ����
        rb.mass = objectMass;           // ���� ����
        rb.drag = 1f;                 // ���� ���� ȿ��
        rb.angularDrag = 0.05f;         // ȸ�� ����
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection.Normalize();

            // ������ ���� �浹 ���ۿ� ����
            float forceMagnitude = objectMass / otherRb.mass;
            rb.AddForce(-forceDirection * forceMagnitude, ForceMode.Impulse);
            otherRb.AddForce(forceDirection * (0.5f / forceMagnitude), ForceMode.Impulse);
        }
    }
}
