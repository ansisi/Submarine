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

    private Vignette vignette;

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
            Debug.LogWarning("PostProcessVolume이 할당되지 않았습니다.");
            return;
        }

        if (postProcessVolume.profile.TryGetSettings<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
        }
        else
        {
            Debug.LogWarning("프로필에 Vignette 효과가 없습니다.");
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
}
