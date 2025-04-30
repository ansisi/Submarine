using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance;

    public AudioClip bgm1;
    public AudioClip bgm2;

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

    private void Update()
    {
        // 테스트용 키 입력
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayBGM(bgm1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayBGM(bgm2);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }
}
