using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("Currency Data")]
    public int gold = 0;

    [Header("UI")]
    public TextMeshProUGUI goldText;

    public event Action OnGoldChanged; // 골드 변경 시 호출

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //LoadCurrency();
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddGold(1000);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke();
        UpdateUI();

        if (gold >= 5000 && WaveManager.Instance.GetCurrentWave() == 2 && !WaveManager.Instance.IsWaveRunning())
        {
            WaveManager.Instance.TriggerWaveStart(); // 세 번째 웨이브 시작
        }

        //SaveCurrency();
    }

    public bool HasGold(int amount)
    {
        return gold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(); // 변경 알림
            UpdateUI();
            //SaveCurrency();
            return true;
        }
        return false;
    }

    public int GetGold()
    {
        return gold;
    }

    public void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"{gold} $";
    }

    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("Gold", gold);
    }

    private void LoadCurrency()
    {
        gold = PlayerPrefs.GetInt("Gold", 0);
    }
}
