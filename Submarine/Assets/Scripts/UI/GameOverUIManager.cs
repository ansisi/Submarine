using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance;  // 싱글톤 인스턴스

    public GameObject gameOverPanel;  // Game Over Panel (검은 배경)
    public Button retryButton;        // Retry 버튼
    public Button quitButton;         // Quit 버튼

    void Awake()
    {
        // 싱글톤 인스턴스를 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // 이미 인스턴스가 있으면 현재 오브젝트를 파괴
        }

        gameOverPanel.SetActive(false);

        // 버튼 클릭 이벤트 연결
        retryButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    // 게임오버 UI 표시
    public void ShowGameOverUI()
    {
        gameOverPanel.SetActive(true);  // UI 활성화
        Time.timeScale = 0f;            // 게임 일시 정지
    }

    // Retry 버튼 클릭 시 첫 번째 빌드로 돌아가기
    void RestartGame()
    {
        // 씬 로드 후 Time.timeScale을 1로 설정하는 리스너 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(0);  // 첫 번째 장면 (빌드 순서 첫 번째)으로 로드
    }

    // 씬이 로드된 후 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드된 후 Time.timeScale을 1로 설정
        Time.timeScale = 1f;

        // 이벤트 리스너 제거 (메모리 관리)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Quit 버튼 클릭 시 게임 종료
    void QuitGame()
    {
        Application.Quit(); // 게임 종료
    }
}
