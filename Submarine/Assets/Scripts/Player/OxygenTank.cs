using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public float maxOxygen = 100f;       // 최대 산소량
    public float currentOxygen;          // 현재 산소량
    public float oxygenRemoveAmount = 10f; // 40초마다 감소할 산소량

    void Start()
    {
        currentOxygen = maxOxygen;
        StartCoroutine(OxygenRemoveRoutine());
    }

    // 산소 감소 코루틴
    IEnumerator OxygenRemoveRoutine()
    {
        while (currentOxygen > 0)
        {
            yield return new WaitForSeconds(40f); // 40초 대기
            currentOxygen -= oxygenRemoveAmount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            Logger.Log("산소 감소: " + currentOxygen);

            if (currentOxygen <= 0)
            {
                GameManager.Instance.GameOver();
                Logger.Log("산소 부족! 게임 오버");
                yield break; // 코루틴 종료
            }
        }
    }

    // 산소 획득 함수
    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        Logger.Log("산소 획득: " + currentOxygen);
    }
}
