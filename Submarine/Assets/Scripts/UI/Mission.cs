using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public GameObject missionImage;
    public Button deleteButton;

    private float deleteTime = 0f;

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
