using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button quitButton;
    public string gameSceneName = "GameScene";

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButton);
        quitButton.onClick.AddListener(OnQuitButton);
    }

    private void OnStartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
