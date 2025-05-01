using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthFillImage;  // 빨간 체력바 이미지
    public Enemy enemy;            // 대상 Enemy 연결

    void Update()
    {
        if (enemy != null && healthFillImage != null)
        {
            float ratio = Mathf.Clamp01(enemy.GetCurrentHealth() / enemy.maxHealth);
            healthFillImage.fillAmount = ratio;
        }
    }
}
