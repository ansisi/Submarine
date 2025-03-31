using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HookUIManager : MonoBehaviour
{
    public Image hookIcon;
    public Image harpoonIcon;

    private Color activeColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 어둡게
    private Color inactiveColor = Color.white; // 기본색

    private void Start()
    {
        if (hookIcon != null)
            hookIcon.color = inactiveColor;

        if (harpoonIcon != null)
            harpoonIcon.color = inactiveColor;
    }

    public void UpdateHookUI(bool isHookActive)
    {
        if (hookIcon != null)
            hookIcon.color = isHookActive ? activeColor : inactiveColor;
    }

    public void UpdateHarpoonUI(bool isHarpoonActive)
    {
        if (harpoonIcon != null)
            harpoonIcon.color = isHarpoonActive ? activeColor : inactiveColor;
    }


}
