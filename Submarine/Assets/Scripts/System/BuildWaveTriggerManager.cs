using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildWaveTriggerManager : MonoBehaviour
{
    public static BuildWaveTriggerManager Instance { get; private set; }    // 싱글턴 인스턴스

    private bool wave1Triggered;  // 1웨이브 트리거 여부
    private bool wave2Triggered;  // 2웨이브 트리거 여부

    [SerializeField]
    private string turretItemName = "Turret";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // PlayerData 인스턴스가 존재하면 업그레이드 변경 이벤트 등록
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.OnUpgradeChanged += OnUpgradeChanged;
        }
    }
    private void OnEnable()
    {
        // Wave1: 포탑 제작 퀘스트 완료 시 (CraftItem 이벤트 감지)
        QuestEventSystem.OnQuestAction += OnQuestActionReceived;

    }

    private void OnDisable()
    {
        QuestEventSystem.OnQuestAction -= OnQuestActionReceived;
        if (PlayerData.Instance != null)
            PlayerData.Instance.OnUpgradeChanged -= OnUpgradeChanged;    
    }

    // 1웨이브 콜백
    private void OnQuestActionReceived(QuestActionType type, string param)
    {
        if (type == QuestActionType.CraftItem && param == turretItemName &&
            !wave1Triggered && WaveManager.Instance.GetCurrentWave() == 0 && !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart();
            wave1Triggered = true;  
            Logger.Log("발표용 1웨이브 시작 (포탑 제작 퀘스트 완료)");
        }
    }

    // 2웨이브 콜백
    private void OnUpgradeChanged(UpgradeType type, int newLevel)
    {
        if (wave2Triggered) return;

        bool allUpgraded = PlayerData.Instance.GetAllUpgradeLevels()
            .All(kv => kv.Value >= 1);

        if (allUpgraded && WaveManager.Instance.GetCurrentWave() == 1 && !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart();
            wave2Triggered = true;  
            Logger.Log("발표용 2웨이브 시작 (모든 업그레이드 레벨1 달성)");
        }
    }
}
