using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public RectTransform oxygenNeedle;     // UI 연료 게이지 바늘 (Inspector에서 할당)
    public float maxOxygenTime = 400f;       // 연료가 다 떨어질 때까지의 최대 시간(초)
    private float elapsedOxygenTime = 0f;    // 경과된 연료 소비 시간

    private float fullOxygenAngle = -177f;
    private float emptyOxygenAngle = 68f;

    private void Start()
    {

    }

    void Update()
    {
        // 매 프레임마다 연료 소비 시간을 누적 (시간 기반 소비)
        elapsedOxygenTime += Time.deltaTime;

        // 연료 잔량 비율 계산 (0 ~ 1)
        float oxygenRatio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);

        // 연료 게이지 바늘 회전 각도 계산
        float oxygenAngle = Mathf.Lerp(fullOxygenAngle, emptyOxygenAngle, oxygenRatio);
        if (oxygenNeedle != null)
        {
            oxygenNeedle.localEulerAngles = new Vector3(0, 0, oxygenAngle);
        }

        // 연료가 다 소비되었을 때 게임 오버 처리 (또는 원하는 임계값 사용)
        if (oxygenRatio >= 1.0f)
        {
            GameManager.Instance.GameOver();
            Debug.Log("연료 부족! 게임 오버");
        }
    }

    public void AddOxygen(float amount)
    {
        // 코루틴을 이용하여 부드럽게 연료 소비 시간을 감소시킵니다.
        StopCoroutine("RefillOxygen");
        StartCoroutine(RefillOxygen(amount));
    }

    private IEnumerator RefillOxygen(float amount)
    {
        float startTime = elapsedOxygenTime;
        float targetTime = Mathf.Max(elapsedOxygenTime - amount, 0f); // 음수가 되지 않도록
        float duration = 1.0f; // 보충 효과 지속 시간
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            elapsedOxygenTime = Mathf.Lerp(startTime, targetTime, t / duration);
            yield return null;
        }
        elapsedOxygenTime = targetTime;
    }
    public bool IsLow()
    {
        float oxygenRatio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);
        float oxygenAngle = Mathf.Lerp(fullOxygenAngle, emptyOxygenAngle, oxygenRatio);
        return oxygenAngle >= -50f;
    }

    public void ConsumeOxygen(float amount)
    {
        elapsedOxygenTime += amount;
        elapsedOxygenTime = Mathf.Clamp(elapsedOxygenTime, 0f, maxOxygenTime);
    }

}
