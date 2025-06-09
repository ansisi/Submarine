using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Player,
    Enemy,
    Neutral
}

public class FactionHandler : MonoBehaviour
{
    public Faction faction;

    public bool IsEnemy(FactionHandler other)
    {
        return faction != other.faction; // 기본 로직: 서로 다르면 적
    }

    public bool IsAlly(FactionHandler other)
    {
        return faction == other.faction;
    }
}
