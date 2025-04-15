using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    public int pickaxeTier = 1; // ÀÌ °î±ªÀÌÀÇ µî±Þ
    public float miningRange = 2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, miningRange);
            foreach (var hit in hits)
            {
                Ore ore = hit.GetComponent<Ore>();
                if (ore != null)
                {
                    if (ore.CanMineWith(pickaxeTier))
                    {
                        ore.Mine();
                    }
                    else
                    {
                        Logger.Log("ÀÌ ±¤¼®Àº ÀÌ °î±ªÀÌ·Î´Â Ä¶ ¼ö ¾ø½À´Ï´Ù.");
                    }
                    break;
                }
            }
        }
    }
}