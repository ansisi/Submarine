using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    [Header("UI References")]
    public RectTransform content;  // QuestPanel
    public GameObject itemPrefab;  // QuestItemPrefab

    private List<GameObject> items = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Setup(Quest quest)
    {
        foreach (var go in items) Destroy(go);
        items.Clear();

        for (int i = 0; i < quest.data.steps.Count; i++)
        {
            var go = Instantiate(itemPrefab, content);
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = quest.data.steps[i].description;

            // 첫 스텝만 해금
            go.transform.Find("LockedOverlay").gameObject
                .SetActive(i != 0);
            go.transform.Find("Checkmark").gameObject
                .SetActive(false);
            items.Add(go);
        }
    }

    public void UpdateOnStepComplete(int newIndex)
    {
        // 이전 스텝 체크 표시
        items[newIndex - 1].transform.Find("Checkmark").gameObject.SetActive(true);
        // 다음 스텝 해금
        if (newIndex < items.Count)
            items[newIndex].transform.Find("LockedOverlay").gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
