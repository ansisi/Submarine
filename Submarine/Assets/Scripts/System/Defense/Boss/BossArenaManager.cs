using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaManager : MonoBehaviour
{
    public static BossArenaManager Instance { get; private set; }

    public GameObject arenaWalls; // 벽 또는 Invisible Collider 오브젝트
    public GameObject extraObjectA;    // 비활성화할 대상 A
    public GameObject extraObjectB;    // 비활성화할 대상 B


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 초기 상태로 벽 비활성화
        DisableArena();
    }

    public void EnableArena()
    {
        arenaWalls.SetActive(true); // 보스전 시작 시 벽 활성화
        DisableExtraObjects();
    }

    public void DisableArena()
    {
        arenaWalls.SetActive(false); // 전투 종료 시 해제
    }

    private void DisableExtraObjects()
    {
        if (extraObjectA != null) extraObjectA.SetActive(false);
        if (extraObjectB != null) extraObjectB.SetActive(false);
    }

}
