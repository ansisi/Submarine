using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineFaul : MonoBehaviour
{
    public float maxFuel = 100f;       // �ִ� ���ᷮ
    public float currentFuel;          // ���� ���ᷮ
    public float fuelRemoveAmount = 10f;   // 60�ʸ��� ������ ���ᷮ

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());
    }

    // ���� ���� �ڷ�ƾ
    IEnumerator FuelRemoveRoutine()
    {
        while (currentFuel > 0)
        {
            yield return new WaitForSeconds(60f); // 60�� ���
            currentFuel -= fuelRemoveAmount;
            currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
            Logger.Log("���� ����: " + currentFuel);

            if (currentFuel <= 0)
            {
                Logger.Log("���� ����! ���� ����");
                yield break; // �ڷ�ƾ ����
            }
        }
    }

    // ���� ���� �Լ�
    public void AddFuel(float amount)
    {
        currentFuel += amount;
        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
        Logger.Log("���� ����: " + currentFuel);
    }
}
