using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject upgradePanel;  // 업그레이드 UI 패널
    public Transform upgradeListContainer;  // 업그레이드 항목들이 표시될 컨테이너
    public GameObject upgradeItemPrefab;  // 업그레이드 항목 프리팹
    public TextMeshProUGUI goldText;  // 골드 표시 UI

    private List<UpgradeData> availableUpgrades;  // 사용할 업그레이드 데이터 목록

    private void Start()
    {
        // 초기화
        availableUpgrades = new List<UpgradeData>(); // 업그레이드 항목 데이터

        // 예시로 업그레이드 데이터들을 미리 등록할 수 있다면, 이를 통해 UI에 표시할 수 있음
        LoadUpgrades();
        UpdateGoldUI();
    }

    // 업그레이드 데이터 로딩
    private void LoadUpgrades()
    {
        // 예시: 게임 내 업그레이드 데이터 리스트 가져오기
        // 이 데이터는 ScriptableObject에서 읽어올 수 있음
        foreach (UpgradeData upgradeData in availableUpgrades)
        {
            CreateUpgradeItem(upgradeData);
        }
    }

    // 업그레이드 항목을 UI에 추가
    private void CreateUpgradeItem(UpgradeData upgradeData)
    {
        GameObject upgradeItem = Instantiate(upgradeItemPrefab, upgradeListContainer);
        UpgradeItemUI itemUI = upgradeItem.GetComponent<UpgradeItemUI>();

        itemUI.SetUpgradeData(upgradeData);
    }

    // 골드 UI 업데이트
    private void UpdateGoldUI()
    {
        goldText.text = CurrencyManager.Instance.GetGold().ToString();
    }

    // 업그레이드 패널 열기
    public void OpenUpgradePanel()
    {
        upgradePanel.SetActive(true);
    }

    // 업그레이드 패널 닫기
    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }
}