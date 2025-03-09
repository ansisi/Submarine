using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // �ִ� ��ǰ ���� ����
    public int maxParts = 5;

    // �ʿ��� ��ǰ ���� ���� (�������� ����)
    public int requiredSteelParts;
    public int requiredScrewNailParts;
    public int requiredSemiconductorParts;


    // ������ ��ǰ ��
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

        // ���� ��ǰ ���� ����
        requiredSteelParts = Random.Range(1, maxParts + 1);
        requiredScrewNailParts = Random.Range(1, maxParts + 1);
        requiredSemiconductorParts = Random.Range(1, maxParts + 1);

        // ��ǰ ���� ��� (����� �α�)
        Logger.Log("�ʿ��� ��ǰ ���� - ö: " + requiredSteelParts + ", �����: " + requiredScrewNailParts + ", �ݵ�ü: " + requiredSemiconductorParts);

        // �ʱ� ��ǰ ���� ����
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0;
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
        //Invoke("LoadNextStage", 2f);
    }

    private void LoadNextStage()
    {
        // ���� ���ÿ� ��ϵ� ���� �������� ���� ���� �ε�
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);  // ���� �� �ε�
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

