using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public string faction; // "Player" 또는 "Enemy"
    public Vector3 direction; // 총알의 이동 방향


    void Start()
    {
        // 방향을 설정해줍니다 (초기에는 기본 방향 설정)
        if (direction == Vector3.zero)
        {
            direction = transform.forward; // 기본적으로 Z 방향
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 대상이 지형 오브젝트라면 총알 제거
        if (other.CompareTag("Terrain"))
        {
            Destroy(gameObject);
            return;
        }

        // 대상에 FactionHandler가 없으면 그냥 통과
        FactionHandler otherFaction = other.GetComponent<FactionHandler>();
        if (otherFaction == null)
        {
            return;  // 아무 일도 하지 않고 통과
        }

        // 대상이 데미지를 받을 수 있고, 진영이 다를 때만
        if (otherFaction.faction != this.faction)
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}