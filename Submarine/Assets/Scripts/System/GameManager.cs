using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool npcRescued = false;

    private bool isGameOver = false;
    
    public bool IsGameOver
    {
        get { return isGameOver; }
        set
        {
            isGameOver = value;
            Time.timeScale = isGameOver ? 0f : 1f;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (isGameOver)
            return;

    }

    public void MarkNPCRescued()
    {
        npcRescued = true;
        Logger.Log("NPC 구조 완료!");
    }

    // 게임 오버 처리
    public void GameOver()
    {
        IsGameOver = true;
        Logger.Log("게임 오버!");
        GameOverUIManager.Instance.ShowGameOverUI();
    }

    public void ResetGame()
    {
        npcRescued = false;
        IsGameOver = false;
    }
}

