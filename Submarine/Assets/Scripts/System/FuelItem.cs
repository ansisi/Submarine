using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelItem : DeliverableItem
{
    public float fuelAmount = 10f; // ����� ä���� ��

    public override void OnDelivered(Submarine submarine)
    {
        submarine.AddFuel(fuelAmount);
        Logger.Log("Fuel delivered: " + fuelAmount);
        Destroy(gameObject); // ���� �� ������Ʈ ����
    }
}
