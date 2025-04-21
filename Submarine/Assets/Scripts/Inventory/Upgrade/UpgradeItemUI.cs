using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image upgradeIcon;  // 업그레이드 아이콘 이미지
    public TextMeshProUGUI upgradeNameText;  // 업그레이드 이름 텍스트
    public TextMeshProUGUI descriptionText;  // 업그레이드 설명 텍스트
    public Button upgradeButton;  // 업그레이드 버튼
    public Transform levelContainer;  // 단계별 UI 요소가 담길 컨테이너
    public GameObject levelPrefab;  // 단계별 UI 프리팹

    private UpgradeData upgradeData;

    // 업그레이드 데이터 설정
    public void SetUpgradeData(UpgradeData data)
    {
        upgradeData = data;
        upgradeIcon.sprite = data.icon;
        upgradeNameText.text = data.upgradeName;
        descriptionText.text = data.description;

        // 단계별 업그레이드 정보 생성
        foreach (var level in data.levels)
        {
            CreateUpgradeLevelUI(level);
        }

        // 업그레이드 버튼 동작 설정
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

    }

    // 업그레이드 단계 UI 생성
    private void CreateUpgradeLevelUI(UpgradeLevelData levelData)
    {
        GameObject levelItem = Instantiate(levelPrefab, levelContainer);
        UpgradeLevelUI levelUI = levelItem.GetComponent<UpgradeLevelUI>();
        levelUI.SetUpgradeLevelData(levelData);
    }

    // 업그레이드 버튼 클릭 시 동작
    private void OnUpgradeButtonClicked()
    {
        // 업그레이드 처리 로직 (예: 골드가 충분한지, 재료가 있는지 확인 후 업그레이드)
        if (CurrencyManager.Instance.SpendGold(upgradeData.levels[0].goldCost))  // 예시로 첫 번째 단계 업그레이드
        {
            // 업그레이드 성공 시 UI 업데이트
            UpdateGoldUI();
        }
    }

    private void UpdateGoldUI()
    {
        CurrencyManager.Instance.UpdateUI();  // 골드 UI 업데이트
    }
}
