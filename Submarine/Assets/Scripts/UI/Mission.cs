using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public static Mission Instance { get; private set; }

    public GameObject missionImage;
    public Button deleteButton;

    private float deleteTime = 0f;

    public GameObject resourceUIPrefab; // 프리팹 연결
    public Transform resourceUIParent;  // UI를 배치할 부모 (예: Vertical Layout Group 사용)

    private Dictionary<PartType, GameObject> resourceUIElements = new Dictionary<PartType, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void SetupMissionUI(List<PartType> missionParts, Dictionary<PartType, int> requiredParts)
    {
        if (Instance == null) return;

        // 기존 UI 요소 제거
        foreach (var ui in Instance.resourceUIElements.Values)
        {
            Destroy(ui);
        }
        Instance.resourceUIElements.Clear();

        // 새로운 부품에 대해 UI 생성
        foreach (var part in missionParts)
        {
            GameObject uiObj = Instantiate(Instance.resourceUIPrefab, Instance.resourceUIParent);
            uiObj.name = part.ToString();

            // 텍스트 설정 (처음에는 0으로 표시)
            TextMeshProUGUI textComponent = uiObj.transform.Find("resourceText").GetComponent<TextMeshProUGUI>();
            textComponent.text = $"0 / {requiredParts[part]}";

            // 아이콘 설정
            Image icon = uiObj.transform.Find("icon").GetComponent<Image>();
            icon.sprite = ResourceManager.Instance.GetPartIcon(part);

            Instance.resourceUIElements[part] = uiObj;
        }
    }

    void Start()
    {
        Time.timeScale = 0;
        missionImage.SetActive(true);
        deleteButton.onClick.AddListener(HideMission);
    }

    void Update()
    {
        deleteTime += Time.deltaTime;

        if (deleteTime > 10f)
        {
            missionImage.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void HideMission()
    {
        missionImage.SetActive(false);
        Time.timeScale = 1;
    }

}
