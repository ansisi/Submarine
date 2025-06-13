using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject optionPanel;

    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButton);
        optionButton.onClick.AddListener(OnOptionButton);
        quitButton.onClick.AddListener(OnQuitButton);
    }


    private void OnStartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnOptionButton()
    {
        optionPanel.SetActive(true);
    }

    private void OnQuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}
