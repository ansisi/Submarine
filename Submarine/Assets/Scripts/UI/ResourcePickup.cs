using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public int resourceType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            ResourceManager.Instance.CollectResource(resourceType);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResourceManager.Instance.CollectResource(resourceType);
    }
}
