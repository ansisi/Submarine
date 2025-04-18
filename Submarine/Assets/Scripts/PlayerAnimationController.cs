using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private float defaultSpeed = 1.0f; // 기본 애니메이션 속도
    private float boostedSpeed = 2.0f; // 스페이스바 누를 때 애니메이션 속도

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isMovingForward = Input.GetKey(KeyCode.W);
        bool isBoosting = Input.GetKey(KeyCode.LeftShift);


        // 앞으로 이동할 때 다리 애니메이션 재생
        animator.SetBool("isSwimming", isMovingForward);

        // A 키를 누르면 오른팔 애니메이션 재생
        animator.SetBool("isPushingRight", Input.GetKey(KeyCode.A));

        // D 키를 누르면 왼팔 애니메이션 재생
        animator.SetBool("isPushingLeft", Input.GetKey(KeyCode.D));

        // 다리 애니메이션 속도만 조절 (LegSpeed 파라미터 사용)
        animator.SetFloat("LegSpeed", isMovingForward ? (isBoosting ? boostedSpeed : defaultSpeed) : defaultSpeed);

        // 마우스 왼쪽 클릭 시 작살 발사 또는 취소
        animator.SetBool("isHarpooning", Input.GetMouseButtonDown(0));

    }
}

