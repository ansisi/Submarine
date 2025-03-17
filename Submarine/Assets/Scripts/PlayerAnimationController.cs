using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 앞으로 이동할 때 다리 애니메이션 재생
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isSwimming", true);
        }
        else
        {
            animator.SetBool("isSwimming", false);
        }

        // A 키를 누르면 오른팔 애니메이션 재생
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isPushingRight", true);
        }
        else
        {
            animator.SetBool("isPushingRight", false);
        }

        // D 키를 누르면 왼팔 애니메이션 재생
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isPushingLeft", true);
        }
        else
        {
            animator.SetBool("isPushingLeft", false);
        }
    }
}
