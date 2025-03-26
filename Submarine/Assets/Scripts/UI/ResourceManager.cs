using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public List<Sprite> partIcons; // 부품별 아이콘 리스트 (Inspector에서 설정)
    private Dictionary<PartType, Sprite> iconDictionary = new Dictionary<PartType, Sprite>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Enum 값과 아이콘 매핑
        PartType[] allParts = (PartType[])System.Enum.GetValues(typeof(PartType));
        for (int i = 0; i < allParts.Length; i++)
        {
            if (i < partIcons.Count)
            {
                iconDictionary[allParts[i]] = partIcons[i];
            }
        }
    }

    public Sprite GetPartIcon(PartType partType)
    {
        return iconDictionary.ContainsKey(partType) ? iconDictionary[partType] : null;
    }
}
