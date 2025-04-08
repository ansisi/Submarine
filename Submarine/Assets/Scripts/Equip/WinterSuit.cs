using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/WinterSuit")]
public class WinterSuit : EquipmentItem
{
    public float resistanceAmount = 0.2f;
    public float effectDuration = 60f;
    public Material warmMaterial; // Inspector에서 따뜻한 머테리얼 지정
    public Material originalMaterial; // 기본 머테리얼 저장 (선택사항)

    public override void Use(GameObject player)
    {
        player.GetComponent<PlayerEquipmentManager>()?.StartCoroutine(ActivateWarmthEffect());
    }

    private IEnumerator ActivateWarmthEffect()
    {
        var tempGimmick = GameObject.FindObjectOfType<TemperatureGimmick>();
        if (tempGimmick != null)
        {
            tempGimmick.ApplyColdResistance(resistanceAmount);
            Logger.Log("[방한복] 체온 저하 저항 적용됨!");

            var warmObjects = GameObject.FindGameObjectsWithTag("WarmObject");
            List<Renderer> renderers = new List<Renderer>();

            foreach (var obj in warmObjects)
            {
                var rend = obj.GetComponent<Renderer>();
                if (rend != null)
                {
                    renderers.Add(rend);
                    originalMaterial = rend.sharedMaterial;
                    rend.sharedMaterial = warmMaterial;
                }
            }

            yield return new WaitForSeconds(effectDuration);

            // 복원
            tempGimmick.ApplyColdResistance(0f);
            foreach (var rend in renderers)
            {
                if (rend != null)
                    rend.sharedMaterial = originalMaterial;
            }
        }
    }
}
