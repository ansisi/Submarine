using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("패널 토글")]
    [SerializeField] private GameObject upgradePanel;  // 전체 UI 루트
    [SerializeField] private Button openButton;        // UI 열기
    [SerializeField] private Button closeButton;       // UI 닫기

    [Header("탭 버튼 (UpgradeData 목록)")]
    [SerializeField] private Button tabButtonPrefab;   // 프리팹: TextMeshProUGUI로 이름 표시
    [SerializeField] private Transform tabButtonParent;// 프리팹을 인스턴스할 부모

    [Header("레벨 버튼들")]
    [SerializeField] private List<Button> levelButtons; // LV1,LV2,LV3 버튼 3개

    [Header("업그레이드 콘텐츠")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text goldCostText;
    [SerializeField] private Button upgradeButton;

    [Header("재료 슬롯")]
    [SerializeField] private List<UpgradeMaterialSlot> materialSlots; // 최대 3칸

    private List<UpgradeData> allUpgrades;
    private UpgradeData currentUpgrade;
    private UpgradeLevelData currentLevelData;
    private int currentLevelIndex;

    private void Awake()
    {
        // 1) 패널 열기/닫기 자동 연결
        openButton.onClick.AddListener(ShowPanel);
        closeButton.onClick.AddListener(HidePanel);
        HidePanel();

        // 2) 모든 UpgradeData 에셋 로드하고, 탭 버튼 자동 생성·리스너 연결
        allUpgrades = new List<UpgradeData>(Resources.LoadAll<UpgradeData>("Upgrades"));
        foreach (var data in allUpgrades)
        {
            var btn = Instantiate(tabButtonPrefab, tabButtonParent);
            btn.GetComponentInChildren<TMP_Text>().text = data.upgradeName;
            btn.onClick.AddListener(() => SetUpgradeData(data));
        }

        // 3) 레벨 버튼들 자동 연결
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int idx = i;
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(idx));
        }

        // 4) 업그레이드 실행 버튼 자동 연결
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }

    /// <summary>
    /// 외부(탭 버튼)에서 호출: 패널이 열려 있고, 해당 업그레이드 데이터로 UI를 초기화
    /// </summary>
    public void SetUpgradeData(UpgradeData data)
    {
        currentUpgrade = data;
        currentLevelIndex = 0;
        RefreshContent();
        ShowPanel();
    }

    private void OnLevelButtonClicked(int levelIdx)
    {
        currentLevelIndex = levelIdx;
        RefreshContent();
        UpdateLevelButtonsState();
    }

    private void RefreshContent()
    {
        if (currentUpgrade == null) return;
        if (currentLevelIndex >= currentUpgrade.levels.Count) return;

        currentLevelData = currentUpgrade.levels[currentLevelIndex];

        // 레벨 0일 경우를 따로 처리합니다.
        if (currentLevelIndex == 0)
        {
            levelText.text = "LV.0";  // 또는 다른 방식으로 표시
        }
        else
        {
            levelText.text = $"LV.{currentLevelData.level}";
        }
        iconImage.sprite = currentUpgrade.icon;
        nameText.text = currentUpgrade.upgradeName;
        descriptionText.text = currentLevelData.levelDescription;
        goldCostText.text = currentLevelData.goldCost.ToString();

        // 재료 슬롯 세팅
        for (int i = 0; i < materialSlots.Count; i++)
        {
            if (i < currentLevelData.materialRequirements.Count)
            {
                materialSlots[i].gameObject.SetActive(true);
                materialSlots[i].Set(currentLevelData.materialRequirements[i]);
            }
            else
            {
                materialSlots[i].Clear();
                materialSlots[i].gameObject.SetActive(false);
            }
        }

        // 업그레이드 버튼 활성화 조건
        bool hasItems = InventoryManager.Instance.HasEnoughItems(currentLevelData.materialRequirements);
        bool hasGold = CurrencyManager.Instance.HasGold(currentLevelData.goldCost);
        upgradeButton.interactable = hasItems && hasGold;
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentLevelData == null) return;
        if (!InventoryManager.Instance.HasEnoughItems(currentLevelData.materialRequirements) ||
            !CurrencyManager.Instance.HasGold(currentLevelData.goldCost) ||
            !PlayerData.Instance.CanUpgradeTo(currentUpgrade.upgradeType, currentLevelData.level))
        {
            Logger.Log("업그레이드 불가");
            return;
        }

        InventoryManager.Instance.ConsumeItems(currentLevelData.materialRequirements);
        CurrencyManager.Instance.SpendGold(currentLevelData.goldCost);
        PlayerData.Instance.SetUpgradeLevel(currentUpgrade.upgradeType, currentLevelData.level);

        // 다음 레벨 이동
        currentLevelIndex++;
        RefreshContent();
        UpdateLevelButtonsState();
    }

    private void UpdateLevelButtonsState()
    {
        int unlocked = PlayerData.Instance.GetUpgradeLevel(currentUpgrade.upgradeType);
        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].interactable = true;  // 모든 버튼을 항상 활성화

            // 현재 선택된 레벨 버튼만 비활성 처리해서 강조
            if (i == currentLevelIndex)
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    private void ShowPanel() => upgradePanel.SetActive(true);
    private void HidePanel() => upgradePanel.SetActive(false);
}

