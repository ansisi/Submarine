using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;               // MainAudioMixer 에셋
    const string MUSIC_PARAM = "MusicVolume";
    const string SFX_PARAM = "SFXVolume";

    [Header("Audio Sources")]
    public AudioSource musicSource;            // 씬에 배치된 BGM 전용 AudioSource
    public AudioSource sfxSource;              // 씬에 배치된 SFX 전용 AudioSource (한 개)

    [Header("BGM Clips")]
    public AudioClip bgm1;
    public AudioClip bgm2;

    [Header("SFX Clips")]
    public List<AudioClip> sfxClips;           // Inspector에 넣을 효과음 리스트

    // 런타임에 이름→클립 매핑
    private Dictionary<string, AudioClip> sfxDict;

    // 믹서 그룹 캐싱
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup sfxGroup;

    private void Awake()
    {
        // --- 싱글톤 설정 ---
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

        // --- SFX 딕셔너리 초기화 ---
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
        {
            if (clip != null && !sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
        }

        // --- 믹서 그룹 찾기 & 할당 ---
        var groups = mainMixer.FindMatchingGroups("Music");
        if (groups.Length > 0) musicGroup = groups[0];
        groups = mainMixer.FindMatchingGroups("SFX");
        if (groups.Length > 0) sfxGroup = groups[0];

        musicSource.outputAudioMixerGroup = musicGroup;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        // 초기 볼륨 세팅 (필요시)
        SetMusicVolume(0.35f);
        SetSFXVolume(1f);
    }

    private void Start()
    {
        // 초기 BGM 재생
        PlayBGM(bgm1);
    }

    // === BGM 제어 ======================================

    /// <summary>
    /// 페이드 전환을 이용해 BGM을 바꿉니다.
    /// </summary>
    public void PlayBGM(AudioClip clip, float fadeTime = 1f, float targetVol = 0.35f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        StartCoroutine(FadeSwitch(musicSource, clip, fadeTime, targetVol));
    }

    /// <summary>즉시 BGM 정지</summary>
    public void StopBGM()
    {
        musicSource.Stop();
    }

    /// <summary>전투 모드 BGM</summary>
    public void StartCombat() => PlayBGM(bgm2);

    /// <summary>수리 모드 BGM</summary>
    public void StartRepair() => PlayBGM(bgm1);

    /// <summary>BGM을 기본 상태로 리셋</summary>
    public void ResetBGM()
    {
        PlayBGM(bgm1);
        SetMusicVolume(0.35f);
    }

    /// <summary>슬라이더(0~1) 값을 디시벨로 변환해 적용</summary>
    public void SetMusicVolume(float linearVolume)
    {
        mainMixer.SetFloat(MUSIC_PARAM,
            Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20);
    }

    // === SFX 제어 =====================================

    /// <summary>
    /// 이름으로 등록된 SFX를 PlayOneShot으로 재생
    /// </summary>
    public void PlaySFX(string clipName, float volume = 1f)
    {
        if (!sfxDict.TryGetValue(clipName, out var clip)) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>슬라이더(0~1) 값을 디시벨로 변환해 적용</summary>
    public void SetSFXVolume(float linearVolume)
    {
        mainMixer.SetFloat(SFX_PARAM,
            Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20);
    }

    // === 유틸: 페이드 스위치 코루틴 =====================

    private IEnumerator FadeSwitch(AudioSource src, AudioClip newClip, float fadeTime, float targetVol)
    {
        // 현재 볼륨(선형) 읽어오기
        float startVol = src.volume;

        // 1) 페이드 아웃
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        src.volume = 0f;
        src.Stop();

        // 2) 클립 교체 후 재생
        src.clip = newClip;
        src.Play();

        // 3) 페이드 인
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(0f, targetVol, t / fadeTime);
            yield return null;
        }
        src.volume = targetVol;
    }

}

