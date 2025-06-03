using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSightTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.SetVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pickup = other.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.SetVisible(false);
        }
    }
}
