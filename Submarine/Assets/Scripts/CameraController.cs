using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;  // �÷��̾��� Transform
    public Vector3 offset;    // �⺻ ������ (ī�޶� ��ġ ����)

    public float deadZoneX = 2f; // X�� Dead Zone ����
    public float deadZoneY = 1f; // Y�� Dead Zone ����
    public float minFollowSpeed = 0.5f; // Dead Zone ���ο��� õõ�� ���󰡴� �ӵ�
    public float maxFollowSpeed = 5f; // Dead Zone �ۿ��� ������ ���󰡴� �ӵ�
    public float speedLerpFactor = 2f; // �ӵ� ��ȭ ���� ����

    private float currentSpeed; // ���� ī�޶� �ӵ�
    private Vector3 velocity = Vector3.zero; // SmoothDamp �ӵ� ���� ����

    void Start()
    {
        currentSpeed = minFollowSpeed; // ó���� õõ�� ���󰡵��� ����
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

        transform.rotation = Quaternion.identity; // ī�޶� ȸ�� ����
    }
}
