using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    // ���� ���� ����
    public float maxFuel = 100f;         // �ִ� ���ᷮ
    public float currentFuel;            // ���� ���ᷮ
    public float fuelRemoveAmount = 10f;   // 60�ʸ��� ������ ���ᷮ

    // ��ǰ ���� ���� (�� ��ǰ�� ������ ����)
    public int steelParts = 0;
    public int screwNailParts = 0;
    public int semiconductorParts = 0;
    

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());
    }

    // ���� ���� �ڷ�ƾ: 60�ʸ��� fuelRemoveAmount ��ŭ ���� ����
    IEnumerator FuelRemoveRoutine()
    {
        while (currentFuel > 0)
        {
            yield return new WaitForSeconds(60f);
            currentFuel -= fuelRemoveAmount;
            currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);
            Logger.Log("���� ����: " + currentFuel);

            if (currentFuel <= 0)
            {
                Logger.Log("���� ����! ���� ����");
                yield break;
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

    // ��ǰ �߰� �Լ�: ��ǰ ������ ���� �ش� ��ǰ ī��Ʈ�� ������Ŵ
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
        Logger.Log("��ǰ �߰�: " + partType);
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
