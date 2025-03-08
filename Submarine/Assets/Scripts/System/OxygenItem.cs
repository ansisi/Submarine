using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenItem : MonoBehaviour
{
    public float oxygenAmount = 20f; // 충전할 산소량

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌 시 산소 충전
        OxygenTank playerOxygen = other.GetComponent<OxygenTank>();
        if (playerOxygen != null)
        {
            playerOxygen.AddOxygen(oxygenAmount);
            Logger.Log("산소 충전: " + oxygenAmount);
            Destroy(gameObject); // 아이템 제거
        }
    }
}
