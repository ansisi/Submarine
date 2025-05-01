using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance;

    public AudioClip bgm1;
    public AudioClip bgm2;
    public float fadeDuration = 1f; // 페이드 효과 시간

    private AudioSource audioSource;


    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayBGM(bgm1);
    }


    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying) return;

        StartCoroutine(FadeBGM(clip));
        audioSource.volume = 0.35f; // 볼륨을 반으로 설정
    }


    private IEnumerator FadeBGM(AudioClip newClip)
    {
        // 현재 재생 중인 BGM이 있다면 페이드 아웃
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;

            // 페이드 아웃 (볼륨을 서서히 0으로 줄이기)
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            audioSource.volume = 0;
            audioSource.Stop(); // 재생 중인 음악을 멈춤
        }

        // 새로운 BGM 설정 후 페이드 인 (볼륨을 서서히 1로 올리기)
        audioSource.clip = newClip;
        audioSource.Play();
        float targetVolume = 0.35f;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
    public void StopBGM()
    {
        audioSource.Stop();
    }

    // 전투 시작 시 호출하는 함수 (외부에서 호출 가능)
    public void StartCombat()
    {
        PlayBGM(bgm2); // 전투 BGM 재생
    }

    // 정비 시간 시작 시 호출하는 함수 (외부에서 호출 가능)
    public void StartRepair()
    {
        PlayBGM(bgm1); // 정비 BGM 재생
    }
}
