using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public float maxOxygen = 100f;       // �ִ� ��ҷ�
    public float currentOxygen;          // ���� ��ҷ�
    public float oxygenRemoveAmount = 10f; // 40�ʸ��� ������ ��ҷ�

    void Start()
    {
        currentOxygen = maxOxygen;
        StartCoroutine(OxygenRemoveRoutine());
    }

    // ��� ���� �ڷ�ƾ
    IEnumerator OxygenRemoveRoutine()
    {
        while (currentOxygen > 0)
        {
            yield return new WaitForSeconds(40f); // 40�� ���
            currentOxygen -= oxygenRemoveAmount;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            Logger.Log("��� ����: " + currentOxygen);

            if (currentOxygen <= 0)
            {
                GameManager.Instance.GameOver();
                Logger.Log("��� ����! ���� ����");
                yield break; // �ڷ�ƾ ����
            }
        }
    }

    // ��� ȹ�� �Լ�
    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        Logger.Log("��� ȹ��: " + currentOxygen);
    }
}
