using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image circleImage;             // UI 원형 이미지 (fillAmount로 채움)
    public GameObject messagePanel;       // 메시지 그룹
    public TextMeshProUGUI messageText;   // 메시지 텍스트

    private enum State { None, Wave, Downtime }
    private State state = State.None;

    private float totalTime;
    private float remainingTime;

    private void OnEnable()
    {
        WaveManager.Instance.OnWaveStarted += OnWaveStarted;
        WaveManager.Instance.OnWaveEnded += OnWaveEnded;
    }

    private void OnDisable()
    {
        WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
        WaveManager.Instance.OnWaveEnded -= OnWaveEnded;
    }

    private void Start()
    {
        HideAll();
    }

    private void Update()
    {
        if (state == State.None) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0f);

        // 남은 비율에 따라 원형 게이지 채우기
        circleImage.fillAmount = remainingTime / totalTime;

        if (remainingTime <= 0f)
        {
            // 타이머 자동 숨김
            HideTimer();
            state = State.None;
        }
    }

    private void OnWaveStarted(int waveIndex)
    {
        totalTime = WaveManager.Instance.waveList[waveIndex].waveDuration;
        remainingTime = totalTime;
        state = State.Wave;

        ShowTimer();
        ShowMessage($"Wave {waveIndex + 1} 시작!");
    }

    private void OnWaveEnded(int waveIndex)
    {
        totalTime = WaveManager.Instance.preparationTime;
        remainingTime = totalTime;
        state = State.Downtime;

        ShowTimer();
        ShowMessage("정비 시간!");
    }

    private void ShowTimer()
    {
        // circleImage가 속해있는 부모 패널(예: TimerPanel) 활성화
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
