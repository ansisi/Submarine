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
    [SerializeField] private TMP_Text currentGoldText;
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
    private Dictionary<UpgradeData, Button> upgradeToTabButton = new(); // 업그레이드 ↔ 버튼 매핑
    private Button currentTabButton; // 현재 선택된 탭 버튼
    private int currentLevelIndex;

    private void Awake()
    {
           
        if (openButton == null)
            Debug.LogWarning("openButton이 null입니다!");
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

            // 캡처 문제 방지를 위해 임시 변수 사용
            UpgradeData capturedData = data;
            Button capturedButton = btn;
            btn.onClick.AddListener(() => SetUpgradeData(capturedData, capturedButton));
            upgradeToTabButton.Add(capturedData, capturedButton); // 매핑 추가
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

    public void SetUpgradeData(UpgradeData data)
    {
        // 버튼은 매핑에서 찾아서 넘김
        if (upgradeToTabButton.TryGetValue(data, out var btn))
        {
            SetUpgradeData(data, btn);
        }
    }

    /// <summary>
    /// 외부(탭 버튼)에서 호출: 패널이 열려 있고, 해당 업그레이드 데이터로 UI를 초기화
    /// </summary>
    private void SetUpgradeData(UpgradeData data, Button tabButton = null)
    {
        currentUpgrade = data;
        int currentPlayerLevel = PlayerData.Instance.GetUpgradeLevel(currentUpgrade.upgradeType);
        currentLevelIndex = Mathf.Clamp(currentPlayerLevel, 0, currentUpgrade.levels.Count - 1);

        // 이전 강조 버튼 복원
        if (currentTabButton != null)
            currentTabButton.interactable = true;

        // 현재 강조 버튼 비활성화
        currentTabButton = tabButton;
        currentTabButton.interactable = false;

        RefreshContent();
        UpdateLevelButtonsState();
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

    // 플레이어의 현재 업그레이드 레벨을 가져와서 표시
        int currentPlayerLevel = PlayerData.Instance.GetUpgradeLevel(currentUpgrade.upgradeType);

        if (currentPlayerLevel == 0)
            levelText.text = "현재 레벨 : LV.0";
        else
            levelText.text = $"현재 레벨 : LV.{currentPlayerLevel}";
        currentGoldText.text = $"보유 골드 : {CurrencyManager.Instance.gold}$";
        iconImage.sprite = currentUpgrade.icon;
        nameText.text = currentUpgrade.upgradeName;
        descriptionText.text = $"효과\n{currentLevelData.levelDescription}"; 

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
        goldCostText.text = $"필요 골드 : {currentLevelData.goldCost}$";
        goldCostText.color = hasGold ? Color.white : Color.red;
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
        UpgradeEffectManager.Instance.ApplyUpgrade(currentUpgrade.upgradeType, currentLevelData.level);

    // 다음 레벨 이동
    currentLevelIndex++;
        RefreshContent();
        UpdateLevelButtonsState();
    }

    private void UpdateTabButtonsState(Button newSelectedButton)
    {
        // 이전에 선택한 탭 버튼이 있으면 다시 활성화
        if (currentTabButton != null)
        {
            currentTabButton.interactable = true;
        }

        // 새로 선택한 버튼 비활성화 (강조)
        if (newSelectedButton != null)
        {
            newSelectedButton.interactable = false;
            currentTabButton = newSelectedButton;
        }
    }

    private void UpdateLevelButtonsState()
    {
        int unlockedLevel = PlayerData.Instance.GetUpgradeLevel(currentUpgrade.upgradeType);

        for (int i = 0; i < levelButtons.Count; i++)
        {
            var button = levelButtons[i];
            var buttonText = button.GetComponentInChildren<TMP_Text>();

            // 기본 텍스트는 LV.1, LV.2, ...
            string baseLabel = $"LV.{i + 1}";

            if (i < unlockedLevel)
            {
                // 이미 달성한 레벨: 비활성화 + 텍스트 변경
                button.interactable = false;
                buttonText.text = $"{baseLabel} (완료)";
            }
            else
            {
                button.interactable = true;
                buttonText.text = baseLabel;
            }

            // 현재 선택 중인 탭은 강조를 위해 별도 비활성 처리
            if (i == currentLevelIndex)
            {
                button.interactable = false;
            }
        }
    }

    private void OnEnable()
    {
        CurrencyManager.Instance.OnGoldChanged += RefreshContent;
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnGoldChanged -= RefreshContent;
    }

    private void ShowPanel()
    {
        upgradePanel.SetActive(true);

        if (currentUpgrade == null && allUpgrades.Count > 0)
        {
            var firstData = allUpgrades[0];
            var firstButton = upgradeToTabButton[firstData];
            SetUpgradeData(firstData, firstButton);
        }
    }

    private void HidePanel() => upgradePanel.SetActive(false);
}

