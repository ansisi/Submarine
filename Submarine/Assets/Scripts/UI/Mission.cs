using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public GameObject missionImage;
    public Button deleteButton;

    private float deleteTime = 0f;

    public TextMeshProUGUI steelText;
    public TextMeshProUGUI screwNailText;
    public TextMeshProUGUI semiconductorText;

    void Start()
    {
        Time.timeScale = 0;
        missionImage.SetActive(true);
        deleteButton.onClick.AddListener(HideMission);

        UpdateMissionText();
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

    void UpdateMissionText()
    {
        if (GameManager.Instance != null)
        {
            steelText.text = $"0 / {GameManager.Instance.requiredSteelParts}";
            screwNailText.text = $"0 / {GameManager.Instance.requiredScrewNailParts}";
            semiconductorText.text = $"0 / {GameManager.Instance.requiredSemiconductorParts}";
        }
    }

}
