using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxParts = 3;            // �ִ� ��ǰ ���� ����
    public int minParts = 2;   // �ּ� ��ǰ ����

    // �ʿ��� ��ǰ ���� ���� (�������� ����)
    public int requiredSteelParts;
    public int requiredScrewNailParts;
    public int requiredSemiconductorParts;


    // ������ ��ǰ ��
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

        // �������� Ŭ���� ���� Ȯ��
        if (CheckStageClear())
        {
            StageClear();
        }
    }

    // �ʿ� ���� ���� �ʱ�ȭ
    private void InitializeParts()
    {
        requiredSteelParts = Random.Range(minParts, maxParts + 1);
        requiredScrewNailParts = Random.Range(minParts, maxParts + 1);
        requiredSemiconductorParts = Random.Range(minParts, maxParts + 1);

        Logger.Log($"���ο� ��ǰ ���� - ö: {requiredSteelParts}, �����: {requiredScrewNailParts}, �ݵ�ü: {requiredSemiconductorParts}");
    }

    // ������ ���� �ڿ� ���� �ʱ�ȭ
    private void ResetCollectedParts()
    {
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0; // ��� ��ǰ ���� �ʱ�ȭ
        }
        Logger.Log("������ ��ǰ �ʱ�ȭ �Ϸ�!");
    }

    // ��ǰ ���� ������Ʈ
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

        int requiredCount = GetRequiredParts(partType);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateResourceUI(partType, collectedParts[partType], requiredCount);
        }

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

    // �������� Ŭ���� üũ
    private bool CheckStageClear()
    {
        return collectedParts[PartType.Steel] >= requiredSteelParts &&
               collectedParts[PartType.ScrewNail] >= requiredScrewNailParts &&
               collectedParts[PartType.Semiconductor] >= requiredSemiconductorParts;
    }

    // �������� Ŭ���� ó��
    private void StageClear()
    {
        isGameOver = true;
        // �������� Ŭ���� UI ǥ��
        Logger.Log("�������� Ŭ����!");
        // ���� ���������� �����ϰų� ���� ����
        Invoke("LoadNextStage", 2f);
    }

    private void LoadNextStage()
    {
        // ��� ���ÿ� ��ϵ� ���� �������� ���� ���� �ε�
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        minParts = Mathf.RoundToInt(minParts * 1.5f); // ���� �������� �ּ� ���� ���� ����
        maxParts = Mathf.RoundToInt(maxParts * 1.5f); // ���� �������� �ִ� ���� ���� ����

        SceneManager.LoadScene(nextSceneIndex);  // ���� �� �ε�
        InitializeParts();      // �ʿ� �ڿ� ���� �ʱ�ȭ
        ResetCollectedParts();  // ���� �ڿ� ���� �ʱ�ȭ
        isGameOver = false;
    }

    // ���� ���� ó��
    public void GameOver()
    {
        isGameOver = true;
        // ���� ���� UI ǥ��
        Logger.Log("���� ����!");
        // ���� ����� �Ǵ� ����
    }
}

