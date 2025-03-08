using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenItem : MonoBehaviour
{
    public float oxygenAmount = 20f; // ������ ��ҷ�

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹 �� ��� ����
        OxygenTank playerOxygen = other.GetComponent<OxygenTank>();
        if (playerOxygen != null)
        {
            playerOxygen.AddOxygen(oxygenAmount);
            Logger.Log("��� ����: " + oxygenAmount);
            Destroy(gameObject); // ������ ����
        }
    }
}
