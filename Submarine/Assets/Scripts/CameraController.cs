using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public Vector3 offset;    // 기본 오프셋 (카메라 위치 조정)

    public float deadZoneX = 2f; // X축 Dead Zone 범위
    public float deadZoneY = 1f; // Y축 Dead Zone 범위
    public float minFollowSpeed = 0.5f; // Dead Zone 내부에서 천천히 따라가는 속도
    public float maxFollowSpeed = 5f; // Dead Zone 밖에서 빠르게 따라가는 속도
    public float speedLerpFactor = 2f; // 속도 변화 보간 정도

    private float currentSpeed; // 현재 카메라 속도
    private Vector3 velocity = Vector3.zero; // SmoothDamp 속도 저장 변수

    void Start()
    {
        currentSpeed = minFollowSpeed; // 처음엔 천천히 따라가도록 설정
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        Vector3 cameraPosition = transform.position;

        bool isOutsideX = Mathf.Abs(player.position.x - cameraPosition.x) > deadZoneX;
        bool isOutsideY = Mathf.Abs(player.position.y - cameraPosition.y) > deadZoneY;

        float targetSpeed = isOutsideX || isOutsideY ? maxFollowSpeed : minFollowSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedLerpFactor * Time.deltaTime);

        transform.position = Vector3.SmoothDamp(cameraPosition, targetPosition, ref velocity, 1f / currentSpeed);

        transform.rotation = Quaternion.identity; // 카메라 회전 고정
    }
}
