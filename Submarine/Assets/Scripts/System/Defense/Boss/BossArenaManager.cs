using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaManager : MonoBehaviour
{
    public GameObject arenaWalls; // 벽 또는 Invisible Collider 오브젝트

    public void EnableArena()
    {
        arenaWalls.SetActive(true); // 보스전 시작 시 벽 활성화
    }

    public void DisableArena()
    {
        arenaWalls.SetActive(false); // 전투 종료 시 해제
    }
}
