using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class OxygenTank : MonoBehaviour
{
    public TextMeshProUGUI oxygenText;               // UI 퍼센트 텍스트 (Inspector에서 할당)
    public float maxOxygenTime = 400f;    // 최대 산소 시간
    private float elapsedOxygenTime = 0f; // 누적 산소 소비 시간

    [SerializeField] private float oxygenEfficiency = 1f; // 1이면 정상, 0.95면 5% 감소

    void Update()
    {
        elapsedOxygenTime += Time.deltaTime * oxygenEfficiency;

        // 비율 계산 (0~1)
        float oxygenRatio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);

        // 퍼센트로 변환
        int oxygenPercent = Mathf.RoundToInt((1f - oxygenRatio) * 100f);

        // 텍스트 UI 갱신
        if (oxygenText != null)
        {
            oxygenText.text = $"{oxygenPercent}%";
        }

        // 산소 부족 시 게임 오버 처리
        if (oxygenRatio >= 1f)
        {
            GameManager.Instance.GameOver();
            Debug.Log("산소 부족! 게임 오버");
        }
    }

    public void AddOxygen(float amount)
    {
        StopCoroutine("RefillOxygen");
        StartCoroutine(RefillOxygen(amount));
    }

    public void SetOxygenEfficiency(float efficiency)
    {
        oxygenEfficiency = Mathf.Clamp(efficiency, 0f, 1f); // 안전하게 제한
    }

    private IEnumerator RefillOxygen(float amount)
    {
        float startTime = elapsedOxygenTime;
        float targetTime = Mathf.Max(elapsedOxygenTime - amount, 0f);
        float duration = 1.0f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            elapsedOxygenTime = Mathf.Lerp(startTime, targetTime, t / duration);

            // 갱신된 퍼센트 표시
            if (oxygenText != null)
            {
                float ratio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);
                int percent = Mathf.RoundToInt((1f - ratio) * 100f);
                oxygenText.text = $"{percent}%";
            }

            yield return null;
        }

        elapsedOxygenTime = targetTime;
    }

    public bool IsLow()
    {
        float ratio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);
        return ratio >= 0.8f; // 20% 이하일 때 경고로 간주
    }

    public void ConsumeOxygen(float amount)
    {
        elapsedOxygenTime += amount;
        elapsedOxygenTime = Mathf.Clamp(elapsedOxygenTime, 0f, maxOxygenTime);
    }

}
