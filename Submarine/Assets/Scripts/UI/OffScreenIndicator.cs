using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    [Header("Target to Track")]
    public Transform target;         // 함선 오브젝트(월드에서 표시할 대상)

    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("Indicator UI")]
    public RectTransform indicatorUI; // 화살표 UI
    public TextMeshProUGUI distanceText;
    public float edgeOffset = 50f;   // 화면 가장자리에서 얼마나 띄울지
    public float rotationOffset = -90f; // 화살표 기본 오프셋, 프리팹이 위를 바라보면 -90

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // 시작 시 Indicator를 숨김
        if (indicatorUI != null)
        {
            indicatorUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (target == null || indicatorUI == null || mainCamera == null)
            return;

        // 월드 좌표 -> 화면 좌표 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 화면 범위 안에 있는지 체크 (카메라 뒤쪽은 고려하지 않음)
        bool isOffScreen = (screenPos.x <= 0 || screenPos.x >= Screen.width ||
                            screenPos.y <= 0 || screenPos.y >= Screen.height);

        // 화면 범위 안에 있으면 인디케이터 끔
        if (!isOffScreen)
        {
            indicatorUI.gameObject.SetActive(false);
            return;
        }

        // 화면 밖이면 인디케이터 켬
        indicatorUI.gameObject.SetActive(true);

        // 거리 계산 및 표시
        float distance = Vector3.Distance(playerTransform.position, target.position);
        distanceText.text = $"{Mathf.RoundToInt(distance)}m"; // 소수점 제거

        // 화면 좌표를 화면 범위로 Clamp (가장자리에 붙도록)
        screenPos.x = Mathf.Clamp(screenPos.x, edgeOffset, Screen.width - edgeOffset);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeOffset, Screen.height - edgeOffset);

        // 인디케이터의 위치 설정
        indicatorUI.position = screenPos;

        // 인디케이터의 회전(화면 중심 -> 대상 방향)
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = (new Vector2(screenPos.x, screenPos.y) - screenCenter).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 프리팹이 위를 바라보고 있으므로, 회전 오프셋을 적용
        indicatorUI.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
