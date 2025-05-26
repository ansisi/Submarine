using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused { get; private set; } = false;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // 1) 메뉴 숨기기
        pauseMenuUI.SetActive(false);

        // 2) 버튼 클릭 리스너 연결
        resumeButton.onClick.AddListener(ResumeGame);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(QuitGame);


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;               // 게임 일시정지
        GameIsPaused = true;

        QuestEventSystem.Raise(QuestActionType.OpenPauseMenu);
    }

    private void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;               // 게임 재개
        GameIsPaused = false;
    }

    private void OpenOptions()
    {
        // TODO: 옵션 서브메뉴 활성화 로직
        Debug.Log("Options 메뉴 열기 (추후 구현)");
    }

    private void QuitGame()
    {
        // 에디터에서는 재생 중지, 빌드에서는 애플리케이션 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
