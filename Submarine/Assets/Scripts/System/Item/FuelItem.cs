using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelItem : DeliverableItem
{
    public float fuelAmount = 10f; // 연료로 채워질 양

    public override void OnDelivered(Submarine submarine)
    {
        submarine.AddFuel(fuelAmount);
        Logger.Log("Fuel delivered: " + fuelAmount);
        Destroy(gameObject); // 전달 후 오브젝트 제거
    }
}
