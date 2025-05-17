using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupMessageUI : MonoBehaviour
{
    public static PopupMessageUI Instance;

    public GameObject PopupPanel;
    public TextMeshProUGUI PopupText;
    public Button closeButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        PopupPanel.SetActive(false);

        // X버튼 클릭 시 창 닫기
        closeButton.onClick.AddListener(HideNotification);
    }

    public void ShowNotification(string message)
    {
        PopupText.text = message;
        PopupPanel.SetActive(true);
    }

    public void HideNotification()
    {
        PopupPanel.SetActive(false);
    }
}
