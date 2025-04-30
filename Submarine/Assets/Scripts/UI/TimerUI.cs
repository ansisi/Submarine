using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image timerImage;  // UI 원 이미지
    public WaveManager waveManager;  // 웨이브 매니저 참조
    private float timeRemaining;  // 남은 시간
    private float totalWaveTime;  // 한 웨이브의 전체 시간 (타이머에 맞게 설정)

    void Start()
    {
        // 웨이브 매니저에서 웨이브의 진행 시간 가져오기
        totalWaveTime = waveManager.waveDuration;
        timeRemaining = totalWaveTime;
    }

    void Update()
    {
        if (waveManager.GetCurrentWave() > 0 && timeRemaining > 0)
        {
            // 타이머 진행
            timeRemaining -= Time.deltaTime;

            // FillAmount를 통해 원의 크기 조정
            float fillAmount = Mathf.Clamp01(timeRemaining / totalWaveTime);
            timerImage.fillAmount = fillAmount;

            if (timeRemaining <= 0)
            {
                // 타이머가 끝났을 때 추가 작업 (예: 웨이브 완료 처리)
                Debug.Log("Wave Time Ended!");
            }
        }
    }

    public void ResetTimer()
    {
        // 타이머 리셋
        timeRemaining = totalWaveTime;
        timerImage.fillAmount = 1f;
    }

    public void SetWaveDuration(float newDuration)
    {
        // 웨이브 지속 시간 변경
        totalWaveTime = newDuration;
        ResetTimer();
    }
}
