using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Transform playerTransform;    // 플레이어의 현재 위치를 참조
    public float maxHookLength = 10f;   // 후크 최대 길이
    public float initialSpeed = 20f;    // 발사 시 초기 속도

    public float gravityPull = 0.8f;    // 최대 길이 도달 후 적용할 중력 효과 (Y축 아래로)
    public float retractSpeed = 15f;    // 후크 당길 때의 속도

    private Rigidbody rb;
    private Vector3 fireDirection;      // 발사 방향 (XY 평면)
    private bool isFired = false;       // 발사 상태 여부
    public bool isRetracting = false;   // 당김 상태 여부

    // 후크가 자원(또는 지형)에 걸렸을 경우 해당 오브젝트 참조 (한 번에 하나만 허용)
    private GameObject attachedObject = null;

    void Awake()
    {

        rb = GetComponent<Rigidbody>();
        // XY 평면 전용: Z축 이동 및 회전 고정
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY;
    }

    // 발사 함수: 플레이어 측에서 호출
    public void Fire(Vector3 direction)
    {
        isFired = true;
        isRetracting = false;
        fireDirection = direction;
        rb.velocity = fireDirection * initialSpeed;
    }

    // 당김 상태 시작 함수: 플레이어 측에서 호출
    public void StartRetraction()
    {
        isRetracting = true;
        isFired = false;
    }

    void Update()
    {
        if (isFired)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance < maxHookLength)
            {
                // 후크가 최대 길이에 도달하기 전까지, 거리 비례로 초기 속도에서 선형 보간(Lerp)하여 감속
                float t = distance / maxHookLength; // 0 ~ 1 사이 값
                float effectiveSpeed = Mathf.Lerp(initialSpeed, 0f, t);
                rb.velocity = fireDirection * effectiveSpeed;
            }
            else
            {
                // 최대 길이에 도달하면, 후크는 더 이상 멀어지지 않고, 중력 효과로 Y축 아래로 이동
                Vector3 clampedPosition = playerTransform.position + (transform.position - playerTransform.position).normalized * maxHookLength;
                transform.position = clampedPosition;
                rb.velocity = new Vector3(0, -gravityPull, 0);
            }
        }
        else if (isRetracting)
        {
            // 당김 상태: 후크가 플레이어 쪽으로 당겨짐
            Vector3 pullDir = (playerTransform.position - transform.position).normalized;
            rb.velocity = pullDir * retractSpeed;
        }
    }

    // 후크가 자원이나 지형(잠수함 포함)과 충돌하면, 단순히 닿은 것으로 판단
    private void OnCollisionEnter(Collision collision)
    {
        if (isFired && attachedObject == null)
        {
            if (collision.gameObject.CompareTag("Resource") || collision.gameObject.CompareTag("Terrain"))
            {
                attachedObject = collision.gameObject;
                // 선택: 오브젝트를 후크에 붙이기 위해 해당 리지드바디의 물리 효과를 비활성화합니다.
                Rigidbody otherRb = attachedObject.GetComponent<Rigidbody>();
                if (otherRb != null)
                {
                    otherRb.isKinematic = true;
                }
                StartRetraction();
            }
        }
    }
}
