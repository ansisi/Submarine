using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEffectManager : MonoBehaviour
{
    public static UpgradeEffectManager Instance { get; private set; }

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

    private void ApplyAllUpgradesFromSave()
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
    private void ApplySpaceshipUpgrade(int level) { /* TODO */ }
    private void ApplyOxygenUpgrade(int level) { /* TODO */ }
    private void ApplyInventoryUpgrade(int level) { /* TODO */ }
    private void ApplyAutoRepairUpgrade(int level) { /* TODO */ }
}
