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

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateUI();
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
