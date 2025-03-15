using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public GameObject hookPrefab;         // 후크 프리팹
    public Transform hookSpawnPoint;      // 후크가 생성될 위치 (일반적으로 플레이어 위치)
    public float grabRange = 0.5f;        // 후크가 플레이어에 도달하면 잡힌 것으로 간주하는 거리
    public LineRenderer lineRenderer;     // 플레이어와 후크 사이를 이어줄 라인

    private GameObject currentHook;

    void Update()
    {
        // 왼쪽 클릭: 후크가 없다면 발사, 있다면 당김 명령 전달
        if (Input.GetMouseButtonDown(0))
        {
            if (currentHook == null)
            {
                // 후크 생성 및 발사
                currentHook = Instantiate(hookPrefab, hookSpawnPoint.position, Quaternion.identity);
                // 마우스 위치를 월드 좌표로 변환 (XY 평면에서 사용할 Z값은 임의 설정)
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f;
                Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3 fireDir = (worldMouse - hookSpawnPoint.position);
                fireDir.z = 0; // XY 평면 제한
                fireDir = fireDir.normalized;

                // 후크 스크립트에 발사 명령 전달
                Hook hookScript = currentHook.GetComponent<Hook>();
                if (hookScript != null)
                {
                    hookScript.Fire(fireDir);
                    hookScript.playerTransform = hookSpawnPoint;
                }
            }
            else
            {
                // 이미 발사된 후크가 있으면, 당김 상태로 전환
                Hook hookScript = currentHook.GetComponent<Hook>();
                if (hookScript != null)
                {
                    hookScript.StartRetraction();
                }
            }
        }

        // 라인 렌더러로 후크와 플레이어 연결 (후크가 있으면 계속 업데이트)
        if (currentHook != null && lineRenderer != null)
        {
            lineRenderer.SetPosition(0, hookSpawnPoint.position);
            lineRenderer.SetPosition(1, currentHook.transform.position);
            lineRenderer.enabled = true;
        }
        else if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // 후크가 플레이어의 잡기 범위 내에 도달하면 후크 제거
        if (currentHook != null)
        {
            Hook hookScript = currentHook.GetComponent<Hook>();
            if (hookScript != null && hookScript.isRetracting)
            {
                float distance = Vector3.Distance(transform.position, currentHook.transform.position);
                if (distance < grabRange)
                {
                    Destroy(currentHook);
                    currentHook = null;
                }
            }
        }
    }

}
