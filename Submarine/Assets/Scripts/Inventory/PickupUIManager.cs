using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupUIManager : MonoBehaviour
{
    public static PickupUIManager instance;

    public GameObject hintPanel;    // "E키로 줍기" 안내 UI
    public TextMeshProUGUI hintText;           // 아이템 이름 포함 안내 텍스트

    private void Awake()
    {
        instance = this;
        ShowHint(false);
    }

    // 힌트 표시/숨기기
    public void ShowHint(bool show, string text = "")
    {
        // text가 비어있으면 show를 강제로 false 처리
        if (string.IsNullOrWhiteSpace(text))
        {
            show = false;
        }

        hintPanel.SetActive(show);
        if (show)
            hintText.text = text;
    }
}
