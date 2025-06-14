using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance { get; private set; }

    [Header("씬에 비활성화된 BossHealthUI 오브젝트")]
    [SerializeField] private GameObject healthUI;
    private BossHealthBar healthBar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        healthUI.SetActive(false);
        healthBar = healthUI.GetComponent<BossHealthBar>();
    }

    /// <summary>보스 등장 시 UI 보여주고, 보스 레퍼런스 주입</summary>
    public void ShowFor(BossController boss)
    {
        healthUI.SetActive(true);
        healthBar.Initialize(boss);
    }

    /// <summary>보스 사망 시 UI 숨기기</summary>
    public void Hide()
    {
        healthUI.SetActive(false);
    }
}
