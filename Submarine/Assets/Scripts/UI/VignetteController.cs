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

    public Color targetColor = Color.red;    // 부족 시 원하는 붉은색
    private Color defaultColor = Color.black; // 기본 색상

    private Vignette vignette;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        // 싱글턴 인스턴스 초기화
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
            Logger.Log("PostProcessVolume이 할당되지 않았습니다.");
            return;
        }

        if (postProcessVolume.profile.TryGetSettings<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
            vignette.color.value = defaultColor;
        }
        else
        {
            Logger.Log("프로필에 Vignette 효과가 없습니다.");
        }
    }

    public void TriggerVignetteEffect()
    {
        StartCoroutine(VignetteRoutine());
    }

    private IEnumerator VignetteRoutine()
    {
        float timer = 0f;
        float initialIntensity = vignette.intensity.value;
        float targetIntensity = 0.5f; // 효과의 목표 intensity 값

        // 페이드 인: 서서히 목표 intensity로 증가
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, timer / fadeDuration);
            yield return null;
        }

        // 목표 상태 유지
        yield return new WaitForSeconds(holdDuration);

        // 페이드 아웃: 원래 상태로 복귀
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetIntensity, initialIntensity, timer / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator ContinuousVignetteRoutine(bool enable)
    {
        float timer = 0f;
        float startIntensity = vignette.intensity.value;
        float targetIntensity = enable ? 0.6f : 0f;
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
    }

    public void UpdateVignetteState(bool isOxygenLow, bool isFuelLow)
    {
        // 부족 상태 중 하나라도 true이면 효과 활성화
        bool shouldEnable = isOxygenLow || isFuelLow;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ContinuousVignetteRoutine(shouldEnable));
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
