using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffectManager : MonoBehaviour
{

    // Script Execution Order: 이 스크립트는 UpgradeEffectManager보다 먼저 실행되어야 합니다.
    public static UpgradeEffectManager Instance { get; private set; }

    [SerializeField] private Spaceship spaceship;
    [SerializeField] private OxygenTank oxygenTank;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        ApplyAllUpgradesFromSave(); // 게임 시작 시 저장된 업그레이드 효과 자동 적용
    }

    
    public void ApplyAllUpgradesFromSave()
    {
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            int level = PlayerData.Instance.GetUpgradeLevel(type); // 저장된 업그레이드 레벨 조회
            if (level > 0)
            {
                ApplyUpgrade(type, level); // 업그레이드 효과 적용
            }
        }
    }

    public void ApplyUpgrade(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.Spaceship:
                ApplySpaceshipUpgrade(level);
                break;

            case UpgradeType.Radar:
                ApplyRadarUpgrade(level);
                break;

            case UpgradeType.OxygenTank:
                ApplyOxygenUpgrade(level);
                break;

            case UpgradeType.Inventory:
                ApplyInventoryUpgrade(level);
                break;

            case UpgradeType.AutoRepair:
                ApplyAutoRepairUpgrade(level);
                break;
        }
    }

    private void ApplyRadarUpgrade(int level)
    {
        if (SpaceshipBoundary.Instance != null)
        {
            SpaceshipBoundary.Instance.SetAntennaUpgradeLevel(level);
        }
        else
        {
            Logger.LogWarning("Radar 업그레이드 실패: SpaceshipBoundary.Instance가 null입니다.");
        }
    }

    // 나머지 업그레이드 함수들은 추후 구현
    private void ApplySpaceshipUpgrade(int level) 
    {
        if (spaceship != null)
        {
            float multiplier;

            switch (level)
            {
                case 1:
                    multiplier = 1.05f; // 5% 증가
                    break;
                case 2:
                    multiplier = 1.07f; // 7% 증가
                    break;
                case 3:
                    multiplier = 1.10f; // 10% 증가
                    break;
                default:
                    Logger.LogWarning("알 수 없는 Spaceship 업그레이드 레벨: " + level);
                    return;
            }

            float baseMaxHealth = spaceship.baseMaxHealth; // 기준 체력 값 사용
            spaceship.maxHealth = baseMaxHealth * multiplier;

        }
        else
        {
            Logger.LogWarning("Spaceship 업그레이드 실패: spaceship 객체가 null입니다.");
        }
    }
    private void ApplyOxygenUpgrade(int level) 
    {
        if (oxygenTank != null)
        {
            float efficiency;

            switch (level)
            {
                case 1:
                    efficiency = 0.97f; // 3% 감소
                    break;
                case 2:
                    efficiency = 0.95f; // 5% 감소
                    break;
                case 3:
                    efficiency = 0.90f; // 10% 감소
                    break;
                default:
                    Logger.LogWarning("알 수 없는 산소 업그레이드 레벨: " + level);
                    return;
            }

            oxygenTank.SetOxygenEfficiency(efficiency);
        }
        else
        {
            Logger.LogWarning("Oxygen 업그레이드 실패: oxygenTank가 null입니다.");
        }
    }

    private void ApplyInventoryUpgrade(int level)
    {
        int newSlotCount;

        switch (level)
        {
            case 1:
                newSlotCount = 12; 
                break;
            case 2:
                newSlotCount = 15; 
                break;
            case 3:
                newSlotCount = 20; 
                break;
            default:
                Logger.LogWarning("알 수 없는 인벤토리 업그레이드 레벨: " + level);
                return;
        }

        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Logger.LogWarning("Inventory 업그레이드 실패: InventoryManager 인스턴스가 존재하지 않습니다.");
            return;
        }

        inventory.UpgradeSlotCount(newSlotCount);
    }
    private void ApplyAutoRepairUpgrade(int level) 
    {
        if (spaceship != null)
        {
            float interval; // 기본값

            switch (level)
            {
                case 1:
                    interval = 20f;
                    break;
                case 2:
                    interval = 15f;
                    break;
                case 3:
                    interval = 10f;
                    break;
                default:
                    Logger.LogWarning("알 수 없는 자동회복 업그레이드 레벨: " + level);
                    return;
            }

            spaceship.SetAutoRepairInterval(interval); // 인터벌만 설정
        }
        else
        {
            Logger.LogWarning("AutoRepair 업그레이드 실패: Spaceship 객체를 찾을 수 없습니다.");
        }
    }
}
