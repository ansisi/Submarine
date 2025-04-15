using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float baseThrust = 0.6f;         // 이동 가속도
    public float boostMultiplier = 2.5f; // 쉬프트를 눌렀을 때의 가속 배율
    public float rotationThrust = 2f; // 회전 가속도 (토크)
    public float linearDrag = 0.1f;   // 이동 저항
    public float angularDrag = 0.5f;  // 회전 저항 (회전에 대한 관성 감속)

    private Rigidbody rb;
    [SerializeField]
    private float currentThrust;

    public OxygenTank oxygenTank; // 산소탱크 연결

    public float boostOxygenConsumptionRate = 2f; // 초당 부스트 산소 소모량

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = linearDrag;         // 이동 감속
        rb.angularDrag = angularDrag; // 회전 감속 (관성 감소)

        currentThrust = baseThrust; // 기본은 100%로 시작 
        
    }

    void FixedUpdate()
    {
        currentThrust = baseThrust;
        

        // 산소 부족 여부
        bool isOxygenLow = (oxygenTank != null && oxygenTank.IsLow());

        // 부스트 가능 여부: 쉬프트 누름 + 산소 충분
        bool isBoosting = Input.GetKey(KeyCode.LeftShift) && !isOxygenLow;

        // 왼쪽 쉬프트를 누르면 부스트 적용
        float thrustToApply = isBoosting ? currentThrust * boostMultiplier : currentThrust;

        // 부스트 중이면 산소 소모
        if (isBoosting && oxygenTank != null)
        {
            oxygenTank.ConsumeOxygen(boostOxygenConsumptionRate * Time.deltaTime);
        }

        // 이동 입력 (WASD)
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(transform.up * thrustToApply, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-transform.up * thrustToApply, ForceMode.Acceleration);
        

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