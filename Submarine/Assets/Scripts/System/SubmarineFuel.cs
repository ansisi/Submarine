using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineFaul : MonoBehaviour
{
    public float maxFuel = 100f;       // 최대 연료량
    public float currentFuel;          // 현재 연료량
    public float fuelRemoveAmount = 10f;   // 60초마다 감소할 연료량

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());
    }

    // 연료 감소 코루틴
    IEnumerator FuelRemoveRoutine()
    {
        while (currentFuel > 0)
        {
            yield return new WaitForSeconds(60f); // 60초 대기
            currentFuel -= fuelRemoveAmount;
            currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
            Logger.Log("연료 감소: " + currentFuel);

            if (currentFuel <= 0)
            {
                Logger.Log("연료 부족! 게임 오버");
                yield break; // 코루틴 종료
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
}
