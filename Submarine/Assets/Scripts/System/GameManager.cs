using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxParts = 3;    // 최대 부품 수량
    public int minParts = 2;    // 최소 부품 수량

    // 필요한 부품 수량 (스테이지마다 새롭게 설정됨)
    public int requiredSteelParts;
    public int requiredScrewNailParts;
    public int requiredSemiconductorParts;


    // 수집된 부품 개수 저장 (각 부품 유형별로 저장)
    public Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();

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
    private void Start()
    {
        // UI 초기화는 Start()에서 실행하여 UIManager가 null이 되는 문제 방지
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateResourceUI(PartType.Steel, 0, requiredSteelParts);
            UIManager.Instance.UpdateResourceUI(PartType.ScrewNail, 0, requiredScrewNailParts);
            UIManager.Instance.UpdateResourceUI(PartType.Semiconductor, 0, requiredSemiconductorParts);
        }
    }

    private void Update()
    {
        if (isGameOver)
            return;

        // 모든 부품이 요구량 이상이면 스테이지 클리어
        if (CheckStageClear())
        {
            StageClear();
        }
    }

    // 새로운 스테이지에서 요구되는 부품 개수를 랜덤으로 설정
    private void InitializeParts()
    {
        requiredSteelParts = Random.Range(minParts, maxParts + 1);
        requiredScrewNailParts = Random.Range(minParts, maxParts + 1);
        requiredSemiconductorParts = Random.Range(minParts, maxParts + 1);

        Logger.Log($"- 새로운 부품 수량 -\n철 : {requiredSteelParts}, 못 : {requiredScrewNailParts}, 반도체 : {requiredSemiconductorParts}");
    }


    // 수집된 부품 초기화 (다음 스테이지로 이동할 때 사용)
    private void ResetCollectedParts()
    {
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0; 
        }
        Logger.Log("수집된 부품 초기화 완료!");
    }

    // 현재 수집한 부품 개수 업데이트
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

        // UI ������Ʈ �߰�
        int requiredCount = GetRequiredParts(partType);
        UIManager.Instance.UpdateResourceUI(partType, collectedParts[partType], requiredCount);

    }


    private int GetRequiredParts(PartType partType)
    {
        switch (partType)
        {
            case PartType.Steel: return requiredSteelParts;
            case PartType.ScrewNail: return requiredScrewNailParts;
            case PartType.Semiconductor: return requiredSemiconductorParts;
            default: return 0;
        }
    }

    // 모든 부품이 요구량을 충족하는지 확인
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
        Logger.Log("스테이지 클리어!");
        // 다음 스테이지로 이동
        Invoke("LoadNextStage", 2f);
    }

    // 다음 스테이지 로드 및 부품 설정 갱신
    private void LoadNextStage()
    {
        // 현재 기준 다음 인덱스 씬으로 넘어감 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        // 배율 증가 적용 (점진적인 난이도 상승)
        minParts = Mathf.RoundToInt(minParts * 1.5f); 
        maxParts = Mathf.RoundToInt(maxParts * 1.5f); 

        SceneManager.LoadScene(nextSceneIndex);  // 다음 씬 로드
        InitializeParts();      // 새로운 부품 요구량 설정
        ResetCollectedParts();  // 수집된 부품 초기화
        isGameOver = false;
    }

    // 게임 오버 처리
    public void GameOver()
    {
        isGameOver = true;
        // ���� ���� UI ǥ��
        Logger.Log("게임 오버!");
        // ���� ����� �Ǵ� ����
    }
}

