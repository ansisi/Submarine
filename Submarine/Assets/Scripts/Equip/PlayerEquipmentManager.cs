using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance;

    public List<EquipmentItem> equippedItems;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 콜백 등록
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // 콜백 제거
        }
    }


    void Start()
    {
        ApplyAllEquipmentEffects(); // 최초 시작 시에도 적용
    }

    public void Equip(EquipmentItem item)
    {
        if(!equippedItems.Contains(item))
        { 
            equippedItems.Add(item);
            item.ApplyEffect(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAllEquipmentEffects(); // 새 씬 로드 시 자동 적용
    }

    public void ApplyAllEquipmentEffects()
    {
        GameObject player = GetPlayerObject();
        if (player == null) return;

        foreach (var item in equippedItems)
        {
            if (item != null)
                item.ApplyEffect(player);
        }
    }
    private GameObject GetPlayerObject()
    {
        return GameObject.FindGameObjectWithTag("Player"); // 플레이어 태그 사용
    }

}
