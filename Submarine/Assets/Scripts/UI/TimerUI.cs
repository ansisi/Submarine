using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image circleImage;             // fillAmount로 채워질 원형 게이지
    public GameObject messagePanel;       // 메시지 표시판 (텍스트 + 배경)
    public TextMeshProUGUI messageText;   // 메시지 텍스트

    private enum Phase { None, Preparation, Wave }
    private Phase phase = Phase.None;

    private float totalTime;
    private float remainingTime;
    private bool isRunning = false;

    private void OnEnable()
    {
        // 웨이브 시작/종료 이벤트만 구독
        WaveManager.Instance.OnPreparationStarted += HandlePreparationStarted;
        WaveManager.Instance.OnWaveStarted += HandleWaveStarted;
        WaveManager.Instance.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        WaveManager.Instance.OnPreparationStarted -= HandlePreparationStarted;
        WaveManager.Instance.OnWaveStarted -= HandleWaveStarted;
        WaveManager.Instance.OnWaveEnded -= HandleWaveEnded;
    }

    private void Start()
    {
        HideAll();
    }

    private void Update()
    {
        // 1) 준비 시간 트리거 감지
        if (!isRunning && Input.GetKeyDown(KeyCode.T))
        {
            StartPreparationPhase();
        }

        // 2) 타이머가 동작 중일 때만 카운트다운
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            // 준비가 끝났거나 웨이브가 끝났을 때
            StopTimer();
            return;
        }

        circleImage.fillAmount = remainingTime / totalTime;
    }

    private void HandlePreparationStarted()
    {
        StartPreparationPhase();
    }

    private void StartPreparationPhase()
    {
        phase = Phase.Preparation;
        totalTime = WaveManager.Instance.preparationTime;
        remainingTime = totalTime;
        isRunning = true;

        ShowTimer();
        ShowMessage($"Wave {WaveManager.Instance.GetCurrentWave() + 1} 준비 시작");
    }

    // 실제 웨이브가 시작될 때 호출되는 이벤트 핸들러
    private void HandleWaveStarted(int waveIndex)
    {
        phase = Phase.Wave;
        totalTime = WaveManager.Instance.waveList[waveIndex].waveDuration;
        remainingTime = totalTime;
        isRunning = true;

        ShowTimer();
        ShowMessage($"Wave {waveIndex + 1} 시작!");
    }

    // 웨이브가 종료됐을 때 호출되는 이벤트 핸들러
    private void HandleWaveEnded(int waveIndex)
    {
        StopTimer();

        var waveData = WaveManager.Instance.waveList[waveIndex];
        if (waveData is BossWaveDataSO bossWaveDataSo)
        {
            return;
        }

        ShowMessage($"탐사 시간!");
    }

    private void StopTimer()
    {
        isRunning = false;
        phase = Phase.None;
        HideTimer();
    }

    private void ShowTimer()
    {
        circleImage.transform.parent.gameObject.SetActive(true);
    }

    private void HideTimer()
    {
        circleImage.transform.parent.gameObject.SetActive(false);
    }

    private void ShowMessage(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(MessageRoutine(msg));
    }

    private IEnumerator MessageRoutine(string msg)
    {
        messageText.text = msg;
        messagePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        messagePanel.SetActive(false);
    }

    private void HideAll()
    {
        circleImage.transform.parent.gameObject.SetActive(false);
        messagePanel.SetActive(false);
    }

}
