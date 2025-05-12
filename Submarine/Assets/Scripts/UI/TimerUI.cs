using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image timerImage;               // UI 원형 이미지
    public WaveManager waveManager;        // 웨이브 매니저 참조

    [Header("웨이브 메시지 UI")]
    public GameObject waveMessageGroup;           // 전체 그룹 (통째로 껐다 켬)
    public TextMeshProUGUI waveMessageText;       // 텍스트만

    private float timeRemaining = 0f;      // 현재 타이머 시간
    private float totalTime = 0f;          // 타이머 전체 시간
    private int lastWave = 0;              // 마지막에 감지한 웨이브
    private bool isDowntime = false;       // 지금이 쉬는 시간인지 여부
    private bool isTimerRunning = false;   // 타이머가 진행 중인지 여부

    void Start()
    {
        lastWave = waveManager.GetCurrentWave(); // 타이머는 시작하지 않음
        isTimerRunning = false;

        waveMessageGroup.SetActive(false);
    }

    void Update()
    {
        int currentWave = waveManager.GetCurrentWave();

        // 웨이브가 새로 시작됐는지 감지
        if (currentWave > lastWave)
        {
            lastWave = currentWave;
            isDowntime = false;
            StartWaveTimer();
            ShowWaveMessage($"Wave {currentWave} 시작!");
        }

        if (!isTimerRunning) return;

        // 타이머 감소
        timeRemaining -= Time.deltaTime;
        float fill = Mathf.Clamp01(timeRemaining / totalTime);
        timerImage.fillAmount = fill;

        if (timeRemaining <= 0f)
        {
            isTimerRunning = false;

            if (!isDowntime)
            {
                // 웨이브 시간 종료 → 쉬는 시간으로 전환
                isDowntime = true;
                StartDowntimeTimer();
                ShowWaveMessage("정비 시간!");
            }
        }
    }

    private void StartWaveTimer()
    {
        totalTime = waveManager.waveList[waveManager.GetCurrentWave()].waveDuration;
        timeRemaining = totalTime;
        isTimerRunning = true;
    }

    private void StartDowntimeTimer()
    {
        totalTime = waveManager.preparationTime;
        timeRemaining = totalTime;
        isTimerRunning = true;
    }

    private void ShowWaveMessage(string message)
    {
        StopAllCoroutines(); // 메시지가 겹치지 않도록
        StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        waveMessageText.text = message;
        waveMessageGroup.SetActive(true);

        yield return new WaitForSeconds(5f); // 2초 동안 표시

        waveMessageGroup.SetActive(false);
    }

}
