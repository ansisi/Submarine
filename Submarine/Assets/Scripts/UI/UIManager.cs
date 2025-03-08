using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI[] resourceTexts;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateResourceUI(int index, int currentAmount)
    {
        resourceTexts[index].text = $"{currentAmount}/{ResourceManager.Instance.requiredResources[index]}";
    }
}
