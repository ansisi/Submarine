using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [System.Serializable]
    public class StageData
    {
        public int stageIndex;
        public List<GameObject> gimmickPrefabs; // 프리팹 리스트
    }

    [Header("스테이지 기믹 설정")]
    public List<StageData> stages = new List<StageData>();

    private List<GameObject> spawnedGimmicks = new List<GameObject>();

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
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearOldGimmicks();
        ApplyStageGimmicks();
    }

    private void ApplyStageGimmicks()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        foreach (StageData data in stages)
        {
            if (data.stageIndex == currentSceneIndex)
            {
                foreach (GameObject prefab in data.gimmickPrefabs)
                {
                    if (prefab != null)
                    {
                        GameObject gimmick = Instantiate(prefab);
                        spawnedGimmicks.Add(gimmick);
                    }
                }

                Logger.Log($"[StageManager] 스테이지 {currentSceneIndex} - 기믹 {data.gimmickPrefabs.Count}개 생성 완료!");
                break;
            }
        }
    }

    private void ClearOldGimmicks()
    {
        foreach (GameObject gimmick in spawnedGimmicks)
        {
            if (gimmick != null)
                Destroy(gimmick);
        }
        spawnedGimmicks.Clear();
    }
}
