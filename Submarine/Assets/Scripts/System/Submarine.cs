using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    // ���� ���� ����
    public float maxFuel = 100f;         // �ִ� ���ᷮ
    public float currentFuel;            // ���� ���ᷮ
    public float fuelRemoveAmount = 10f;   // 60�ʸ��� ������ ���ᷮ

    // ��ǰ ���� ���� ���� (��ǰ�� ī��Ʈ)
    private Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();

    void Start()
    {
        currentFuel = maxFuel;
        StartCoroutine(FuelRemoveRoutine());

        // ��ǰ �ʱ�ȭ (�ʿ��� ��ǰ ���� ���⼭ ����)
        foreach (PartType part in System.Enum.GetValues(typeof(PartType)))
        {
            collectedParts[part] = 0;
        }
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
                GameManager.Instance.GameOver();
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
        if (collectedParts.ContainsKey(partType))
        {
            collectedParts[partType]++;
        }
        else
        {
            collectedParts[partType] = 1;
        }

        Logger.Log($"��ǰ �߰�: {partType} - {collectedParts[partType]}��");
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
