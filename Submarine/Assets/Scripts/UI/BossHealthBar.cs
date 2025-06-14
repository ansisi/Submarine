using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;      // Fill Image (Type=Filled)
    private BossController boss; // BossController ÂüÁ¶

    public void Initialize(BossController bossController)
    {
        boss = bossController;
    }

    void Update()
    {
        if (boss == null) return;
        float ratio = Mathf.Clamp01(boss.CurrentHealth / boss.MaxHealth);
        healthFillImage.fillAmount = ratio;
    }
}
