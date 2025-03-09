using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    Steel,
    ScrewNail,
    Semiconductor
}

public class PartItem : DeliverableItem
{
    public PartType partType; // �� ��ǰ�� Ÿ��

    public override void OnDelivered(Submarine submarine)
    {
        submarine.AddPart(partType);
        Logger.Log("Part delivered: " + partType);
        Destroy(gameObject); // ���� �� ������Ʈ ����
    }
}
