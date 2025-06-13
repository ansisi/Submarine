using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettings : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle windowedToggle;

    private Resolution[] resolutions;

    private const string KEY_RES_INDEX = "resolutionIndex";
    private const string KEY_IS_FULLSCREEN = "isFullscreen";

    void Start()
    {
        // 1) 사용 가능한 해상도 목록 불러오기
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // 2) "1920 x 1080" 등 문자열 리스트 생성
        var options = resolutions
            .Select(r => $"{r.width} x {r.height}")
            .Distinct()
            .ToList();

        resolutionDropdown.AddOptions(options);

        // 3) 이전에 저장된 값 불러오기
        int savedResIndex = PlayerPrefs.GetInt(KEY_RES_INDEX, options.Count - 1);
        bool isFullscreen = PlayerPrefs.GetInt(KEY_IS_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;

        resolutionDropdown.value = Mathf.Clamp(savedResIndex, 0, options.Count - 1);

        fullscreenToggle.isOn = isFullscreen;
        windowedToggle.isOn = !isFullscreen;

        ApplyResolution(savedResIndex);
        ApplyFullscreen(isFullscreen);

        // 4) UI 이벤트 연결
        resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        windowedToggle.onValueChanged.AddListener(OnWindowedToggled);
    }

    public void ApplyResolution(int index, bool isFull)
    {
        var parts = resolutionDropdown.options[index].text.Split('x');
        int w = int.Parse(parts[0].Trim()), h = int.Parse(parts[1].Trim());

        Screen.SetResolution(w, h, Screen.fullScreen);
        PlayerPrefs.SetInt(KEY_RES_INDEX, index);
        PlayerPrefs.SetInt(KEY_IS_FULLSCREEN, isFull ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ApplyResolution(int index)
    {
        ApplyResolution(index, fullscreenToggle.isOn);
    }

    public void ApplyFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt(KEY_IS_FULLSCREEN, isFull ? 1 : 0);
        PlayerPrefs.Save();

        ApplyResolution(resolutionDropdown.value, isFull);
    }

    void OnFullscreenToggled(bool isOn)
    {
        if (isOn)
        {
            ApplyFullscreen(true);
        }
    }

    void OnWindowedToggled(bool isOn)
    {
        if (isOn)
        {
            ApplyFullscreen(false);
        }
    }
}
