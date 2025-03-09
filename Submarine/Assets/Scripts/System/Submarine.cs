using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    // 연료 관련 변수
    public float maxFuel = 100f;         // 최대 연료량
    public float currentFuel;            // 현재 연료량
    public float fuelRemoveAmount = 10f;   // 60초마다 감소할 연료량

    // 부품 관련 변수 (각 부품의 개수를 추적)
    public int steelParts = 0;
    public int screwNailParts = 0;
    public int semiconductorParts = 0;
    

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());
    }

    // 연료 감소 코루틴: 60초마다 fuelRemoveAmount 만큼 연료 감소
    IEnumerator FuelRemoveRoutine()
    {
        while (currentFuel > 0)
        {
            yield return new WaitForSeconds(60f);
            currentFuel -= fuelRemoveAmount;
            currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
            Logger.Log("연료 감소: " + currentFuel);

            if (currentFuel <= 0)
            {
                Logger.Log("연료 부족! 게임 오버");
                yield break;
            }
        }
    }

    // 연료 보충 함수
    public void AddFuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        Logger.Log("연료 보충: " + currentFuel);
    }

    // 부품 추가 함수: 부품 종류에 따라 해당 부품 카운트를 증가시킴
    public void AddPart(PartType partType)
    {
        switch (partType)
        {
            case PartType.Steel:
                steelParts++;
                break;
            case PartType.ScrewNail:
                screwNailParts++;
                break;
            case PartType.Semiconductor:
                semiconductorParts++;
                break;
            
        }
        Logger.Log("부품 추가: " + partType);
    }

    private void OnTriggerEnter(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && item.isGrabbed)
        {
            item.OnDelivered(this);
        }
    }
}
