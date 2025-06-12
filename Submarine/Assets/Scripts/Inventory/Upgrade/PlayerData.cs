using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 플레이어의 업그레이드 레벨 데이터를 관리하는 싱글톤.
/// 각 UpgradeType 별 현재 달성 레벨을 저장/로드하고, 업그레이드 가능 여부를 판단.
/// </summary>
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    public event Action<UpgradeType, int> OnUpgradeChanged;

    // UpgradeType → 현재 레벨 (0이면 아직 1레벨도 달성 전)
    private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllUpgradeLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        // 개발용 초기화 키
        if (Input.GetKeyDown(KeyCode.K))
        {
            ResetAllUpgradeLevels();
        }
    }

    /// <summary>
    /// 각 UpgradeType 별로 PlayerPrefs에서 레벨을 불러와서 초기화.
    /// 키: "Upgrade_{타입이름}_Level"
    /// </summary>
    private void LoadAllUpgradeLevels()
    {
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            string key = GetPrefsKey(type);
            int lvl = PlayerPrefs.GetInt(key, 0);
            upgradeLevels[type] = lvl;
        }
    }

    public void ResetAllUpgradeLevels()
    {
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            string key = GetPrefsKey(type);
            PlayerPrefs.DeleteKey(key);
            upgradeLevels[type] = 0; // 딕셔너리 값도 초기화
        }

        PlayerPrefs.Save();
        Logger.Log("모든 업그레이드 레벨이 초기화되었습니다.");
    }


    /// <summary>
    /// 특정 UpgradeType의 현재 레벨 조회. (0 이상)
    /// </summary>
    public int GetUpgradeLevel(UpgradeType type)
    {
        if (upgradeLevels.TryGetValue(type, out int lvl))
            return lvl;
        return 0;
    }

    /// <summary>
    /// level 만큼 업그레이드 완료 후 저장.
    /// UI에서 업그레이드 성공 시 이 메서드를 호출하세요.
    /// </summary>
    public void SetUpgradeLevel(UpgradeType type, int level)
    {
        upgradeLevels[type] = level;
        PlayerPrefs.SetInt(GetPrefsKey(type), level);
        PlayerPrefs.Save();

        OnUpgradeChanged?.Invoke(type, level); // 업그레이드 변경 이벤트 호출
    }

    /// <summary>
    /// 모든 업그레이드 정보를 복사해서 반환 (읽기 전용)
    /// </summary>
    public Dictionary<UpgradeType, int> GetAllUpgradeLevels()
    {
        return new Dictionary<UpgradeType, int>(upgradeLevels); // 얕은 복사
    }

    /// <summary>
    /// 특정 레벨(level)로 업그레이드 가능 여부 판단.
    /// 업그레이드는 항상 가능하도록 수정.
    /// </summary>
    public bool CanUpgradeTo(UpgradeType type, int level)
    {
        return true;
    }

    /// <summary>
    /// PlayerPrefs에 사용할 키 생성 헬퍼.
    /// </summary>
    private string GetPrefsKey(UpgradeType type)
    {
        return $"Upgrade_{type}_Level";
    }
}
