using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    public RectTransform oxygenNeedle;
    public RectTransform fuelNeedle;

    public float maxOxygenTime = 400f;
    public float maxFuelTime = 360f;

    private float elapsedOxygenTime = 0f;
    private float elapsedFuelTime = 0f;

    private float fullOxygenAngle = -140f;
    private float emptyOxygenAngle = 140f;

    private float fullFuelAngle = -140f;
    private float emptyFuelAngle = 140f;

    void Update()
    {
        elapsedOxygenTime += Time.deltaTime;
        elapsedFuelTime += Time.deltaTime;

        float oxygenRatio = Mathf.Clamp01(elapsedOxygenTime / maxOxygenTime);
        float oxygenAngle = Mathf.Lerp(fullOxygenAngle, emptyOxygenAngle, oxygenRatio);
        oxygenNeedle.localEulerAngles = new Vector3(0, 0, oxygenAngle);

        float fuelRatio = Mathf.Clamp01(elapsedFuelTime / maxFuelTime);
        float fuelAngle = Mathf.Lerp(fullFuelAngle, emptyFuelAngle, fuelRatio);
        fuelNeedle.localEulerAngles = new Vector3(0, 0, fuelAngle);

        if (fuelAngle >= 140f || oxygenAngle >= 140f)
        {
            GameOver();
        }
    }

    public void AddFuel(float amount)
    {
        StopCoroutine("RefillFuel");
        StartCoroutine(RefillFuel(amount));
    }

    private IEnumerator RefillFuel(float amount)
    {
        float targetTime = Mathf.Max(elapsedFuelTime - amount, 0);
        float startTime = elapsedFuelTime;

        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            elapsedFuelTime = Mathf.Lerp(startTime, targetTime, elapsed / duration);
            yield return null;
        }

        elapsedFuelTime = targetTime;
    }

    public void AddOxygen(float amount)
    {
        //elapsedOxygenTime = Mathf.Max(elapsedOxygenTime - amount, 0);
        StopCoroutine("RefillOxygen");
        StartCoroutine(RefillOxygen(amount));
    }

    private IEnumerator RefillOxygen(float amount)
    {
        float targetTime = Mathf.Max(elapsedOxygenTime - amount, 0);
        float startTime = elapsedOxygenTime;

        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            elapsedOxygenTime = Mathf.Lerp(startTime, targetTime, elapsed / duration);
            yield return null;
        }

        elapsedOxygenTime = targetTime;
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
