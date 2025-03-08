using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int[] collectedResources = new int[3];
    public int[] requiredResources = { 3, 3, 3 };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectResource(int resourceIndex)
    {
        collectedResources[resourceIndex]++;
        UIManager.Instance.UpdateResourceUI(resourceIndex, collectedResources[resourceIndex]);
        CheckStageClear();
    }

    private void CheckStageClear()
    {
        for (int i = 0; i < collectedResources.Length; i++)
        {
            if (collectedResources[i] < requiredResources[i])
                return;
        }
        Debug.Log("스테이지 클리어!");
    }
}
