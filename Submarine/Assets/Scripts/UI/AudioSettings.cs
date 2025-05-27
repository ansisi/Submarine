using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider bgmSlider;

    private const string KEY_BGM_VOLUME = "bgmVolume";

    private void Start()
    {
        // 1) 이전에 저장된 값 불러오기 (없으면 0.35 사용)
        float saved = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 0.35f);

        // 2) 슬라이더 초기화
        bgmSlider.value = saved;

        // 3) BgmManager 에도 초기 볼륨 적용
        if (BgmManager.Instance != null)
            BgmManager.Instance.SetVolume(saved);

        // 4) 슬라이더 값 변경 시 콜백 등록
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
    }

    private void OnBgmSliderChanged(float value)
    {
        // 값이 바뀔 때마다 즉시 반영
        if (BgmManager.Instance != null)
            BgmManager.Instance.SetVolume(value);

        // 플레이어 설정 저장
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, value);
        PlayerPrefs.Save();
    }
}
