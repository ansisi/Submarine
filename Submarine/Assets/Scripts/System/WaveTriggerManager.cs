using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveTriggerManager : MonoBehaviour
{
    public static WaveTriggerManager Instance { get; private set; }

    [Header("트리거용 아이템/업그레이드 참조")]
    [SerializeField] private Item shieldTurretItem;      // 웨이브4

    private bool wave1Triggered;
    private bool wave2Triggered;
    private bool wave3Triggered;
    private bool wave4Triggered;
    private bool wave5Triggered;
    private bool wave6Triggered;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 각종 이벤트 구독
        QuestEventSystem.OnAllQuestsFinished += OnAllQuestsFinished;     // 웨이브1
        SpaceshipEntrance.OnSpaceshipClosed += OnSpaceshipClosed;        // 웨이브2
        CurrencyManager.OnGoldChanged += OnGoldChanged;                  // 웨이브3
        InventoryManager.Instance.OnItemAdded += OnItemAdded;            // 웨이브4
        PlayerData.Instance.OnUpgradeChanged += OnUpgradeChanged;    // 웨이브5,6
    }

    private void OnDestroy()
    {
        // 이벤트 언구독
        QuestEventSystem.OnAllQuestsFinished -= OnAllQuestsFinished;
        SpaceshipEntrance.OnSpaceshipClosed -= OnSpaceshipClosed;
        CurrencyManager.OnGoldChanged -= OnGoldChanged;
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemAdded -= OnItemAdded;
        PlayerData.Instance.OnUpgradeChanged -= OnUpgradeChanged;
    }

    // 웨이브 1: 퀘스트 끝나면
    private void OnAllQuestsFinished()
    {
        if (!wave1Triggered && WaveManager.Instance.GetCurrentWave() == 0)
        {
            WaveManager.Instance.TriggerWaveStart();
            wave1Triggered = true;
            Logger.Log("웨이브1 시작 (퀘스트 완료)");
        }
    }

    // 웨이브 2: 우주선 UI 닫힐 때, 업그레이드 레벨2 이상
    private void OnSpaceshipClosed()
    {
        if (!wave2Triggered &&
            PlayerData.Instance.GetUpgradeLevel(UpgradeType.Spaceship) >= 2 && WaveManager.Instance.GetCurrentWave() == 1 &&
            !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart();
            wave2Triggered = true;
            Logger.Log("웨이브2 시작 (스페이스쉽 업그레이드 레벨2)");
        }
    }

    // 웨이브 3: 골드 5000 이상
    private void OnGoldChanged(int currentGold)
    {
        if (!wave3Triggered &&
            currentGold >= 5000 && WaveManager.Instance.GetCurrentWave() == 2 &&
            !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart();
            wave3Triggered = true;
            Logger.Log("웨이브3 시작 (골드 5000)");
        }
    }

    // 웨이브 4: 쉴드터렛 아이템 2개 이상
    private void OnItemAdded(Item item)
    {
        if (!wave4Triggered &&
            item == shieldTurretItem &&
            InventoryManager.Instance.CountItem(shieldTurretItem) >= 2 && WaveManager.Instance.GetCurrentWave() == 3 &&
            !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart();
            wave4Triggered = true;
            Logger.Log("웨이브4 시작 (쉴드터렛 2개)");
        }
    }

    // 웨이브 5 & 6: 업그레이드 조건
    private void OnUpgradeChanged(UpgradeType type, int newLevel)
    {
        // 웨이브5: 전체 업그레이드(인벤토리 제외) 6개
        if (!wave5Triggered)
        {
            int totalUpgrades = PlayerData.Instance.GetAllUpgradeLevels()
                                  .Where(keyValuePair => keyValuePair.Key != UpgradeType.Inventory)
                                  .Sum(keyValuePair => keyValuePair.Value);

            if (!wave6Triggered &&
                totalUpgrades >= 6 && WaveManager.Instance.GetCurrentWave() == 4 &&
                !WaveManager.Instance.IsWaveRunning())
            {
                WaveManager.Instance.TriggerWaveStart();
                wave5Triggered = true;
                Logger.Log("웨이브5 시작 (업그레이드 6개 완료)");
            }
        }

        // 웨이브6: 선체강화 & 자동수리 레벨3

        if (!wave6Triggered)
        {
            bool spaceshipUpgraded = PlayerData.Instance.GetUpgradeLevel(UpgradeType.Spaceship) >= 3;
            bool autoRepairUpgraded = PlayerData.Instance.GetUpgradeLevel(UpgradeType.AutoRepair) >= 3;

            if (spaceshipUpgraded && autoRepairUpgraded && WaveManager.Instance.GetCurrentWave() == 5 &&
                !WaveManager.Instance.IsWaveRunning())
            {
                WaveManager.Instance.TriggerWaveStart();
                wave6Triggered = true;
                Logger.Log("웨이브6 시작 (선체강화/자동수리 레벨3)");
            }
        }
    }
}
