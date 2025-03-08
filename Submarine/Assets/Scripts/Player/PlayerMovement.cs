using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float thrust = 5f;         // �̵� ���ӵ�
    public float rotationThrust = 2f; // ȸ�� ���ӵ� (��ũ)
    public float linearDrag = 0.1f;   // �̵� ����
    public float angularDrag = 0.5f;  // ȸ�� ���� (ȸ���� ���� ���� ����)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = linearDrag;         // �̵� ����
        rb.angularDrag = angularDrag; // ȸ�� ���� (���� ����)
    }

    void FixedUpdate()
    {
        // �̵� �Է� (WASD)
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.up * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-transform.up * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-transform.right * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(transform.right * thrust, ForceMode.Acceleration);

        // ȸ�� �Է� (Q, E) - ���� ����
        float rotationInput = 0;
        if (Input.GetKey(KeyCode.Q))
            rotationInput = 1;
        if (Input.GetKey(KeyCode.E))
            rotationInput = -1;

        // ȸ�� ���� ����
        rb.AddTorque(Vector3.forward * rotationInput * rotationThrust, ForceMode.Acceleration);
    }
}
