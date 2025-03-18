using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxParts = 3;            // 최대 부품 수량 설정
    public int minParts = 2;   // 최소 부품 수량

    // 필요한 부품 수량 설정 (랜덤으로 설정)
    public int requiredSteelParts;
    public int requiredScrewNailParts;
    public int requiredSemiconductorParts;


    // 수집된 부품 수
    private Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();

    private bool isGameOver = false;

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

        InitializeParts();

        ResetCollectedParts();

    }

    private void Update()
    {
        if (isGameOver)
            return;

        // 스테이지 클리어 조건 확인
        if (CheckStageClear())
        {
            StageClear();
        }
    }

    // 필요 수량 랜덤 초기화
    private void InitializeParts()
    {
        requiredSteelParts = Random.Range(minParts, maxParts + 1);
        requiredScrewNailParts = Random.Range(minParts, maxParts + 1);
        requiredSemiconductorParts = Random.Range(minParts, maxParts + 1);

        Logger.Log($"새로운 부품 수량 - 철: {requiredSteelParts}, 나사못: {requiredScrewNailParts}, 반도체: {requiredSemiconductorParts}");
    }

    // 수집한 누적 자원 개수 초기화
    private void ResetCollectedParts()
    {
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0; // 모든 부품 수량 초기화
        }
        Logger.Log("수집된 부품 초기화 완료!");
    }

    // 부품 상태 업데이트
    public void UpdateCollectedParts(PartType partType, int count)
    {
        if (collectedParts.ContainsKey(partType))
        {
            collectedParts[partType] = count;
        }
        else
        {
            collectedParts[partType] = count;
        }
    }

    // 스테이지 클리어 체크
    private bool CheckStageClear()
    {
        return collectedParts[PartType.Steel] >= requiredSteelParts &&
               collectedParts[PartType.ScrewNail] >= requiredScrewNailParts &&
               collectedParts[PartType.Semiconductor] >= requiredSemiconductorParts;
    }

    // 스테이지 클리어 처리
    private void StageClear()
    {
        isGameOver = true;
        // 스테이지 클리어 UI 표시
        Logger.Log("스테이지 클리어!");
        // 다음 스테이지로 진행하거나 게임 종료
        Invoke("LoadNextStage", 2f);
    }

    private void LoadNextStage()
    {
        // 빌드 세팅에 등록된 씬을 기준으로 다음 씬을 로드
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        minParts = Mathf.RoundToInt(minParts * 1.5f); // 다음 스테이지 최소 수량 배율 증가
        maxParts = Mathf.RoundToInt(maxParts * 1.5f); // 다음 스테이지 최대 수량 배율 증가

        SceneManager.LoadScene(nextSceneIndex);  // 다음 씬 로드
        InitializeParts();      // 필요 자원 수량 초기화
        ResetCollectedParts();  // 수집 자원 수량 초기화
        isGameOver = false;
    }

    // 게임 오버 처리
    public void GameOver()
    {
        isGameOver = true;
        // 게임 오버 UI 표시
        Logger.Log("게임 오버!");
        // 게임 재시작 또는 종료
    }
}

