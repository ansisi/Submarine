using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject uiImage;

    void Start()
    {
        uiImage.SetActive(true);

        StartCoroutine(HideImage());
    }

    IEnumerator HideImage()
    {
        float elapsedTime = 0f;
        float duration = 3f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        uiImage.SetActive(false);
    }

}
