using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextStageUIManager : MonoBehaviour
{
    public static NextStageUIManager Instance;
    public GameObject nextStagePanel; // 스테이지 클리어 UI 패널
    public Button nextStageButton; // 다음 스테이지 버튼

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        nextStagePanel.SetActive(false); // 초기에는 비활성화
        nextStageButton.onClick.AddListener(CallNextStage);
    }

    // 스테이지 클리어 UI 활성화
    public void ShowNextStageUI()
    {
        nextStagePanel.SetActive(true);
    }

    // 다음 스테이지 로드
    void CallNextStage()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.LoadNextStage();
    }

    // 씬이 로드된 후 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
