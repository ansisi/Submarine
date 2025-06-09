using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public Faction faction; // "Player" 또는 "Enemy"
    public Vector3 direction; // 총알의 이동 방향
    public float lifeTime = 10f; // 총알이 존재할 최대 시간 (초)

    private float lifeTimer = 0f; // 생성 후 경과 시간
    private Collider ownerCollider; // 발사한 포탑의 콜라이더


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
        // 총알 방향대로 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(90f, 0f, 0f);
        transform.rotation = targetRotation;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 발사자 자신은 무시
        if (other == ownerCollider)
        {
            return;
        }
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

    // 발사할 때 Turret 쪽에서 호출해줍니다
    public void Initialize(Vector3 direction, Collider owner)
    {
        this.direction = direction;
        this.ownerCollider = owner;

        if (ownerCollider != null)
        {
            Collider bulletCollider = GetComponent<Collider>();
            if (bulletCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, ownerCollider, true);

                // 0.1초 후 다시 충돌 허용
                StartCoroutine(ReenableCollision(bulletCollider, ownerCollider, 0.1f));
            }
        }
    }

    private IEnumerator ReenableCollision(Collider bulletCollider, Collider ownerCollider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bulletCollider != null && ownerCollider != null)
        {
            Physics.IgnoreCollision(bulletCollider, ownerCollider, false);
        }
    }
}