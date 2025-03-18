using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HookUIManager : MonoBehaviour
{
    public Image hookIcon; // 인스펙터에서 UI 아이콘을 연결하기 위한 변수
    public Image hookBack;
    public Sprite hookActiveSprite; // 후크 사용 중
    public Sprite hookInactiveSprite; // 후크 미사용
    public Sprite hookActiveBack;
    public Sprite hookInactiveBack;

    private void Start()
    {
        if (hookIcon != null)
        {
            hookIcon.sprite = hookInactiveSprite;
            hookBack.sprite = hookInactiveBack;
        }
    }

    public void UpdateHookUI(bool isHookActive)
    {
        if (hookIcon != null)
        {
            if (isHookActive)
            {
                hookIcon.sprite = hookActiveSprite;
                hookBack.sprite = hookActiveBack;
            }
            else
            {
                hookIcon.sprite = hookInactiveSprite;
                hookBack.sprite = hookInactiveBack;
            }
                
        }
    }


}
