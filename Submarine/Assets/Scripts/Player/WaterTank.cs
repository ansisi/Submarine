using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaterTank : MonoBehaviour
{
    public TextMeshProUGUI waterText;               // UI 퍼센트 텍스트 (Inspector에서 할당)
    public float maxWaterTime = 400f;    // 최대 산소 시간
    private float elapsedWaterTime = 0f; // 누적 산소 소비 시간

    void Update()
    {
        elapsedWaterTime += Time.deltaTime;

        // 비율 계산 (0~1)
        float WaterRatio = Mathf.Clamp01(elapsedWaterTime / maxWaterTime);

        // 퍼센트로 변환
        int WaterPercent = Mathf.RoundToInt((1f - WaterRatio) * 100f);

        // 텍스트 UI 갱신
        if (waterText != null)
        {
            waterText.text = $"{WaterPercent}%";
        }

        // 산소 부족 시 게임 오버 처리
        if (WaterRatio >= 1f)
        {
            GameManager.Instance.GameOver();
            Logger.Log("물 부족! 게임 오버");
        }
    }

    public void AddWater(float amount)
    {
        StopCoroutine("RefillWater");
        StartCoroutine(RefillWater(amount));
    }

    private IEnumerator RefillWater(float amount)
    {
        float startTime = elapsedWaterTime;
        float targetTime = Mathf.Max(elapsedWaterTime - amount, 0f);
        float duration = 1.0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            elapsedWaterTime = Mathf.Lerp(startTime, targetTime, t / duration);

            // 갱신된 퍼센트 표시
            if (waterText != null)
            {
                float ratio = Mathf.Clamp01(elapsedWaterTime / maxWaterTime);
                int percent = Mathf.RoundToInt((1f - ratio) * 100f);
                waterText.text = $"{percent}%";
            }

            yield return null;
        }

        elapsedWaterTime = targetTime;
    }

    public bool IsLow()
    {
        float ratio = Mathf.Clamp01(elapsedWaterTime / maxWaterTime);
        return ratio >= 0.8f; // 20% 이하일 때 경고로 간주
    }

    public void ConsumeWater(float amount)
    {
        elapsedWaterTime += amount;
        elapsedWaterTime = Mathf.Clamp(elapsedWaterTime, 0f, maxWaterTime);
    }

}
