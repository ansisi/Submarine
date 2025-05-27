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

    private Resolution[] resolutions;

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
        int savedResIndex = PlayerPrefs.GetInt("resolutionIndex", options.Count - 1);
        bool isFullscreen = PlayerPrefs.GetInt("isFullscreen", Screen.fullScreen ? 1 : 0) == 1;

        resolutionDropdown.value = Mathf.Clamp(savedResIndex, 0, options.Count - 1);
        fullscreenToggle.isOn = isFullscreen;

        ApplyResolution(savedResIndex);
        ApplyFullscreen(isFullscreen);

        // 4) UI 이벤트 연결
        resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
    }

    public void ApplyResolution(int index)
    {
        // 중복 제거했으니 실제 Resolution 배열과 인덱스가 다를 수 있음
        var optionText = resolutionDropdown.options[index].text;
        var parts = optionText.Split('x');
        int w = int.Parse(parts[0].Trim()), h = int.Parse(parts[1].Trim());

        Screen.SetResolution(w, h, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("isFullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
