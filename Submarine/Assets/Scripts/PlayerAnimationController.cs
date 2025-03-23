using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public PlayerPickup playerPickup;
    private float defaultSpeed = 1.0f; // 기본 애니메이션 속도
    private float boostedSpeed = 2.0f; // 스페이스바 누를 때 애니메이션 속도
    
    public Transform itemTransform; // 아이템 위치
    public float handIKWeight = 1.0f; // 손 IK 강도
    public float handOffset = 0.15f; // 양손 간격 조절 값 (이 값을 조절하면 손이 더 벌어짐)

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isMovingForward = Input.GetKey(KeyCode.W);
        bool isBoosting = Input.GetKey(KeyCode.LeftShift);
        bool isGrabbing = playerPickup != null && playerPickup.IsGrabbing;

        // 앞으로 이동할 때 다리 애니메이션 재생
        animator.SetBool("isSwimming", isMovingForward);

        // A 키를 누르면 오른팔 애니메이션 재생
        animator.SetBool("isPushingRight", Input.GetKey(KeyCode.A) && !isGrabbing);

        // D 키를 누르면 왼팔 애니메이션 재생
        animator.SetBool("isPushingLeft", Input.GetKey(KeyCode.D) && !isGrabbing);

        // 다리 애니메이션 속도만 조절 (LegSpeed 파라미터 사용)
        animator.SetFloat("LegSpeed", isMovingForward ? (isBoosting ? boostedSpeed : defaultSpeed) : defaultSpeed);

        // 마우스 오른쪽 클릭 시 후크 발사 또는 취소
        animator.SetBool("isHooking", Input.GetMouseButtonDown(1) && !isGrabbing);

        // 마우스 왼쪽 클릭 시 작살 발사 또는 취소
        animator.SetBool("isHarpooning", Input.GetMouseButtonDown(0) && !isGrabbing);

    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator && playerPickup != null && playerPickup.IsGrabbing) // 마우스 오른쪽 버튼을 누르고 있을 때만 실행
        {
            // 아이템 위치를 기준으로 오른손/왼손 위치 조정
            Vector3 rightHandPos = itemTransform.position - itemTransform.right * handOffset; // 오른쪽으로 이동
            Vector3 leftHandPos = itemTransform.position + itemTransform.right * handOffset;  // 왼쪽으로 이동

            // 오른손 설정
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos);

            // 왼손 설정
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);
        }
        else
        {
            // 손을 원래 위치로 되돌림 (IK 영향 제거)
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }
}

