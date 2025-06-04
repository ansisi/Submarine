using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatusUI : MonoBehaviour
{
    public Material shipMaterial;
    [Range(0f, 1f)] public float hp = 1f;
    public bool isRepairing = false;

    void Update()
    {
        shipMaterial.SetFloat("_HPAmount", hp);
        shipMaterial.SetFloat("_Repairing", isRepairing ? 1f : 0f);
    }
}
