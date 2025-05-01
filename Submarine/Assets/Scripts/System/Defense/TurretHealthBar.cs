using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretHealthBar : MonoBehaviour
{
    public Image healthFillImage; // HealthBar_Fill 연결
    public Turret turret;         // 터렛 본체 연결

    private void Update()
    {
        if (turret != null && healthFillImage != null)
        {
            float ratio = turret.CurrentDurabilityRatio();
            healthFillImage.fillAmount = ratio;
        }
    }
}
