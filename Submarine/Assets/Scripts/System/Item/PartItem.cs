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
    public PartType partType; // 이 부품의 타입

    public override void OnDelivered(Submarine submarine)
    {
        submarine.AddPart(partType);
        Logger.Log("Part delivered: " + partType);
        Destroy(gameObject); // 전달 후 오브젝트 제거
    }
}
