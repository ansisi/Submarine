using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float baseThrust = 5f;         // 이동 가속도
    public float boostMultiplier = 2.5f; // 쉬프트를 눌렀을 때의 가속 배율
    public float rotationThrust = 2f; // 회전 가속도 (토크)
    public float linearDrag = 0.1f;   // 이동 저항
    public float angularDrag = 0.5f;  // 회전 저항 (회전에 대한 관성 감속)

    private Rigidbody rb;
    private float currentThrust;

    public TemperatureGimmick temperatureSystem;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = linearDrag;         // 이동 감속
        rb.angularDrag = angularDrag; // 회전 감속 (관성 감소)

        currentThrust = baseThrust; // 기본은 100%로 시작 (온도 시스템 꺼져있을 수 있으므로)
    }

    void FixedUpdate()
    {

        // 온도 시스템이 활성화된 경우에만 감속 계산
        if (temperatureSystem != null && temperatureSystem.enabled)
        {
            // 기본 10% 감소
            float baseReduction = 0.9f;

            // 체온 20% 감소마다 5% 추가 감소
            float coldRatio = temperatureSystem.GetColdRatio();
            int steps = Mathf.FloorToInt(coldRatio / 0.2f);
            float extraReduction = 1f - (steps * 0.05f);

            currentThrust = baseThrust * baseReduction * extraReduction;
        }
        else
        {
            // 온도 시스템이 비활성화면 원래 속도
            currentThrust = baseThrust;
        }

        // 왼쪽 쉬프트를 누르면 부스트 적용
        float thrustToApply = Input.GetKey(KeyCode.LeftShift) ? currentThrust * boostMultiplier : currentThrust;

        // 이동 입력 (WASD)
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.up * currentThrust, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-transform.up * currentThrust, ForceMode.Acceleration);
        

        // 회전 입력 (Q, E) - 관성 적용
        float rotationInput = 0;
        if (Input.GetKey(KeyCode.A))
            rotationInput = 1;
        if (Input.GetKey(KeyCode.D))
            rotationInput = -1;

        // 회전 관성 적용
        rb.AddTorque(Vector3.forward * rotationInput * rotationThrust, ForceMode.Acceleration);
    }
}