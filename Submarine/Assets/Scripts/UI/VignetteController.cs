using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteController : MonoBehaviour
{
    public static VignetteController Instance { get; private set; }

    public PostProcessVolume postProcessVolume;
    public float fadeDuration = 1.5f;
    public float holdDuration = 2f;

    public Color targetColor = Color.red;
    private Color defaultColor = Color.black;

    private Vignette vignette;
    private Coroutine currentCoroutine;

    private enum VignetteMode { None, WarningEffect, StatusEffect }
    private VignetteMode currentMode = VignetteMode.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (postProcessVolume == null)
        {
            Debug.LogWarning("PostProcessVolume이 할당되지 않았습니다.");
            return;
        }

        if (postProcessVolume.profile.TryGetSettings<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
            vignette.color.value = defaultColor;
        }
        else
        {
            Debug.LogWarning("프로필에 Vignette 효과가 없습니다.");
        }
    }

    public void TriggerVignetteEffect()
    {
        if (vignette == null)
            return;

        // 상태 효과 중이면 폭탄 효과를 잠시 무시
        if (currentMode == VignetteMode.StatusEffect)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentMode = VignetteMode.WarningEffect;
        currentCoroutine = StartCoroutine(VignetteRoutine());
    }

    private IEnumerator VignetteRoutine()
    {
        float timer = 0f;
        float initialIntensity = vignette.intensity.value;
        float targetIntensity = 0.5f;

        // 페이드 인
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, timer / fadeDuration);
            yield return null;
        }

        // 유지
        yield return new WaitForSeconds(holdDuration);

        // 페이드 아웃
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, timer / fadeDuration);
            yield return null;
        }

        vignette.intensity.value = 0f;
        vignette.color.value = defaultColor;
        currentMode = VignetteMode.None;
        currentCoroutine = null;
    }

    private IEnumerator ContinuousVignetteRoutine(bool enable)
    {
        float timer = 0f;
        float startIntensity = vignette.intensity.value;
        float targetIntensity = enable ? 0.4f : 0f;
        Color startColor = vignette.color.value;
        Color endColor = enable ? targetColor : defaultColor;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timer / fadeDuration);
            vignette.color.value = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
        vignette.color.value = endColor;

        currentMode = enable ? VignetteMode.StatusEffect : VignetteMode.None;
        currentCoroutine = null;
    }

    public void UpdateVignetteState(bool isOxygenLow, bool isFuelLow)
    {
        if (vignette == null)
            return;

        bool shouldEnable = isOxygenLow || isFuelLow;

        // 폭탄 효과 중이면 조금 기다렸다가 다시 시도
        if (currentMode == VignetteMode.WarningEffect)
        {
            StartCoroutine(WaitAndRetryUpdate(shouldEnable));
            return;
        }

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ContinuousVignetteRoutine(shouldEnable));
    }

    private IEnumerator WaitAndRetryUpdate(bool targetEnable)
    {
        yield return new WaitForSeconds(2f); // 폭탄 효과 끝날 때까지 대기
        UpdateVignetteState(targetEnable, targetEnable);
    }

    private void OnDestroy()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

}
