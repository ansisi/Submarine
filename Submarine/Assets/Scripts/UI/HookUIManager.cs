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

    public Image harpoonIcon;
    public Image harpoonBack;
    public Sprite harpoonActiveSprite;
    public Sprite harpoonInactiveSprite;
    public Sprite harpoonActiveBack;
    public Sprite harpoonInactiveBack;

    private void Start()
    {
        if (hookIcon != null)
        {
            hookIcon.sprite = hookInactiveSprite;
            hookBack.sprite = hookInactiveBack;
        }

        if (harpoonBack != null)
        {
            harpoonBack.sprite = harpoonInactiveBack;
            harpoonIcon.sprite = harpoonInactiveSprite;
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

    public void UpdateHarpoonUI(bool isHarpoonActive)
    {
        if(harpoonIcon != null)
        {
            if(isHarpoonActive)
            {
                harpoonIcon.sprite = harpoonActiveSprite;
                harpoonBack.sprite = harpoonActiveBack;
            }

            else
            {
                harpoonIcon.sprite = harpoonInactiveSprite;
                harpoonBack.sprite= harpoonInactiveBack;
            }
        }
    }


}
