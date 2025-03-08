using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float thrust = 5f;         // 이동 가속도
    public float rotationThrust = 2f; // 회전 가속도 (토크)
    public float linearDrag = 0.1f;   // 이동 저항
    public float angularDrag = 0.5f;  // 회전 저항 (회전에 대한 관성 감속)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = linearDrag;         // 이동 감속
        rb.angularDrag = angularDrag; // 회전 감속 (관성 감소)
    }

    void FixedUpdate()
    {
        // 이동 입력 (WASD)
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.up * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-transform.up * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-transform.right * thrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(transform.right * thrust, ForceMode.Acceleration);

        // 회전 입력 (Q, E) - 관성 적용
        float rotationInput = 0;
        if (Input.GetKey(KeyCode.Q))
            rotationInput = 1;
        if (Input.GetKey(KeyCode.E))
            rotationInput = -1;

        // 회전 관성 적용
        rb.AddTorque(Vector3.forward * rotationInput * rotationThrust, ForceMode.Acceleration);
    }
}
