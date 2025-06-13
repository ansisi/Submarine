using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaButtonFix : MonoBehaviour
{
    void Awake()
    {
        // 이 오브젝트에 Image 컴포넌트가 있으면 알파 히트 테스트 적용
        var img = GetComponent<Image>();
        if (img != null)
            img.alphaHitTestMinimumThreshold = 0.1f; // 10% 이상 불투명만 클릭
    }
}
