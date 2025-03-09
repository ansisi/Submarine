using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    // 연료 관련 변수
    public float maxFuel = 100f;         // 최대 연료량
    public float currentFuel;            // 현재 연료량
    public float fuelRemoveAmount = 10f;   // 60초마다 감소할 연료량

    // 부품 수집 관련 변수 (부품의 카운트)
    private Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());

        // 부품 초기화 (필요한 부품 수는 여기서 설정)
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0;
        }
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
                GameManager.Instance.GameOver();
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
        if (collectedParts.ContainsKey(partType))
        {
            collectedParts[partType]++;
        }
        else
        {
            collectedParts[partType] = 1;
        }

        Logger.Log($"부품 추가: {partType} - {collectedParts[partType]}개");
        GameManager.Instance.UpdateCollectedParts(partType, collectedParts[partType]);
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
