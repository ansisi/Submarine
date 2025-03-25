using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject resourceUIPrefab; // 프리팹 연결
    public Transform resourceUIParent;  // UI를 배치할 부모 (예: Vertical Layout Group 사용)

    private Dictionary<PartType, GameObject> resourceUIElements = new Dictionary<PartType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 미션이 바뀔 때 UI 초기화
    public void SetupResourceUI(List<PartType> missionParts, Dictionary<PartType, int> requiredParts)
    {
        foreach (var ui in resourceUIElements.Values)
        {
            Destroy(ui);
        }
        resourceUIElements.Clear();

        foreach (var part in missionParts)
        {
            GameObject uiObj = Instantiate(resourceUIPrefab, resourceUIParent);
            uiObj.name = part.ToString();

            // 텍스트 설정
            TextMeshProUGUI textComponent = uiObj.transform.Find("resourceText").GetComponent<TextMeshProUGUI>();
            textComponent.text = $"0 / {requiredParts[part]}";

            // 아이콘 설정 추가!
            Image icon = uiObj.transform.Find("icon").GetComponent<Image>();
            icon.sprite = ResourceManager.Instance.GetPartIcon(part);

            resourceUIElements[part] = uiObj;
        }
    }

    // 특정 부품 UI 업데이트
    public void UpdateResourceUI(PartType partType, int currentAmount, int requiredAmount)
    {
        if (resourceUIElements.ContainsKey(partType))
        {
            TextMeshProUGUI textComponent = resourceUIElements[partType].transform.Find("resourceText").GetComponent<TextMeshProUGUI>();
            textComponent.text = $"{currentAmount} / {requiredAmount}";
        }
    }
}
