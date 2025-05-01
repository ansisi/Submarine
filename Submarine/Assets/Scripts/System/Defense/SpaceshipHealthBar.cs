using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipHealthBar : MonoBehaviour
{
    public Image healthFillImage;   // 빨간색 체력 바 이미지
    public Spaceship spaceship;     // 우주선 본체 연결

    void Update()
    {
        if (spaceship != null && healthFillImage != null)
        {
            float ratio = Mathf.Clamp01(spaceship.GetCurrentHealth() / spaceship.maxHealth);
            healthFillImage.fillAmount = ratio;
        }
    }
}
