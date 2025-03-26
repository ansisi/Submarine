using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxParts = 3;    // 최대 부품 수량
    public int minParts = 2;    // 최소 부품 수량

    private List<PartType> missionParts = new List<PartType>(); // 미션 부품 리스트
    private Dictionary<PartType, int> requiredParts = new Dictionary<PartType, int>(); // 미션 부품 개수


    // 수집된 부품 개수 저장 (각 부품 유형별로 저장)
    public Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();

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

        SelectMissionParts();
        ResetCollectedParts();
    }

    
    private void Start()
    {
        // UI 초기화는 Start()에서 실행하여 UIManager가 null이 되는 문제 방지
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetupResourceUI(missionParts, requiredParts);
        }

        Mission.SetupMissionUI(missionParts, requiredParts);
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

        if (UIManager.Instance != null)
        {
            foreach (var part in missionParts)
            {
                UIManager.Instance.UpdateResourceUI(part, collectedParts[part], requiredParts[part]);
            }
        }

    }

    // 5개 부품 중 3개를 랜덤으로 선택하고 목표 개수 설정
    private void SelectMissionParts()
    {
        List<PartType> allParts = new List<PartType>((PartType[])System.Enum.GetValues(typeof(PartType)));
        System.Random random = new System.Random();
        missionParts = allParts.OrderBy(x => random.Next()).Take(3).ToList(); // 3개 랜덤 선택

        requiredParts.Clear();
        foreach (var part in missionParts)
        {
            requiredParts[part] = Random.Range(minParts, maxParts + 1); // 각 부품의 목표 개수 설정
        }

        Logger.Log("선택된 미션 부품: " + string.Join(", ", missionParts));
    }


    // 수집된 부품 초기화 (다음 스테이지로 이동할 때 사용)
    private void ResetCollectedParts()
    {
        collectedParts.Clear();
        foreach (var part in missionParts)
        {
            collectedParts[part] = 0; 
        }
        Logger.Log("수집된 부품 초기화 완료!");
    }

    // 현재 수집한 부품 개수 업데이트
    public void UpdateCollectedParts(PartType partType, int count)
    {
        if (!collectedParts.ContainsKey(partType)) return;

        collectedParts[partType] = count;
        int requiredCount = GetRequiredParts(partType);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateResourceUI(partType, collectedParts[partType], requiredCount);
        }

    }


    private int GetRequiredParts(PartType partType)
    {
        return requiredParts.ContainsKey(partType) ? requiredParts[partType] : 0;
    }

    // 모든 부품이 요구량을 충족하는지 확인
    private bool CheckStageClear()
    {
        return missionParts.All(part => collectedParts[part] >= requiredParts[part]);
    }

    // 스테이지 클리어 처리
    private void StageClear()
    {
        IsGameOver = true;
        Logger.Log("스테이지 클리어!");
        // 다음 스테이지로 이동
        NextStageUIManager.Instance.ShowNextStageUI();
    }

    // 다음 스테이지 로드 및 부품 설정 갱신
    public void LoadNextStage()
    {
        // 현재 기준 다음 인덱스 씬으로 넘어감 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        // 배율 증가 적용 (점진적인 난이도 상승)
        minParts = Mathf.RoundToInt(minParts * 1.5f); 
        maxParts = Mathf.RoundToInt(maxParts * 1.5f); 

        SceneManager.LoadScene(nextSceneIndex);  // 다음 씬 로드
        SelectMissionParts();       // 새로운 부품 요구량 설정
        ResetCollectedParts();  // 수집된 부품 초기화
        IsGameOver = false;
    }

    public List<PartType> GetMissionParts()
    {
        return missionParts;
    }
    // 게임 오버 처리
    public void GameOver()
    {
        IsGameOver = true;
        // ���� ���� UI ǥ��
        Logger.Log("게임 오버!");
        GameOverUIManager.Instance.ShowGameOverUI();
        // ���� ����� �Ǵ� ����
    }
}

