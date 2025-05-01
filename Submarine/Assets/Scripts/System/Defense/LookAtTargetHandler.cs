using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTargetHandler : MonoBehaviour
{
    public Transform modelTransform; // 모델 회전 처리용 자식 트랜스폼
    public float rotationSpeed = 360f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.z = 0f;

        if (dir == Vector3.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (dir.x < 0f)
            angle += 180f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 360f);

        // 모델 방향 설정
        if (modelTransform != null)
        {
            bool isEnemy = gameObject.layer == LayerMask.NameToLayer("Enemy");

            if (dir.x < 0f)
            {
                //왼쪽을 볼 때
                modelTransform.localRotation = isEnemy
                    ? Quaternion.Euler(40f, 180f, 0f) : Quaternion.Euler(0f, 180f, 0f); // 적 : 터렛
            }
            else
            {
                //오른쪽을 볼 때
                modelTransform.localRotation = isEnemy
                    ? Quaternion.Euler(-40f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);  // 적 : 터렛
            }
        }
    }
}
