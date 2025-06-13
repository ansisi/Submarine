using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsTabManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button screenTabButton;
    public Button audioTabButton;
    public Button controlsTabButton;

    [Header("Tab Contents")]
    public GameObject screenContent;
    public GameObject audioContent;
    public GameObject controlsContent;

    private void Start()
    {
        // 버튼에 리스너 등록
        screenTabButton.onClick.AddListener(() => ShowTab(Tab.Screen));
        audioTabButton.onClick.AddListener(() => ShowTab(Tab.Audio));
        controlsTabButton.onClick.AddListener(() => ShowTab(Tab.Controls));

        // 초기 탭 (예: 화면)
        ShowTab(Tab.Screen);
    }

    enum Tab { Screen, Audio, Controls }

    void ShowTab(Tab tab)
    {
        // 모든 콘텐츠와 버튼 비활성(비선택) 처리
        screenContent.SetActive(false);
        audioContent.SetActive(false);
        controlsContent.SetActive(false);

        // 버튼 색상 초기화 (옵션)
        ResetTabButtonVisuals();

        // 선택된 탭만 활성화 및 버튼 강조
        switch (tab)
        {
            case Tab.Screen:
                screenContent.SetActive(true);
                HighlightButton(screenTabButton);
                break;
            case Tab.Audio:
                audioContent.SetActive(true);
                HighlightButton(audioTabButton);
                break;
            case Tab.Controls:
                controlsContent.SetActive(true);
                HighlightButton(controlsTabButton);
                break;
        }
    }

    void ResetTabButtonVisuals()
    {
        // Color Tint 설정을 써도 되고, 직접 이미지 색을 바꿔도 됩니다.
        UnhighlightButton(screenTabButton);
        UnhighlightButton(audioTabButton);
        UnhighlightButton(controlsTabButton);
    }

    void HighlightButton(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = Color.white;      // 선택된 버튼은 화이트
        btn.colors = colors;
    }

    void UnhighlightButton(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = new Color(0.7f, 0.7f, 0.7f); // 비선택은 회색
        btn.colors = colors;
    }
}
