using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllButtonSFX : MonoBehaviour
{
    private void Awake()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX("button06");
            });
        }
    }
}
