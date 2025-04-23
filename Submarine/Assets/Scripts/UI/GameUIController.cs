using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance;

    [SerializeField] private GameObject npcUpgradeButton;
    

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockNPCUpgradeUI()
    {
        if (npcUpgradeButton != null)
        {
            npcUpgradeButton.SetActive(true);
        }
    }

    
}
