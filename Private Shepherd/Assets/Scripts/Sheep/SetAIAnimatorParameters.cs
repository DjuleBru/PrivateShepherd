using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAIAnimatorParameters : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private SheepMovement sheepMovement;

    Vector2 moveDir;

    private void FixedUpdate() {

        if (sheepMovement.GetReachedEndOfPath()) {
            animator.SetBool("isMoving", false);
        } else {
            moveDir = sheepMovement.GetMoveDir();
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", moveDir.x);
            animator.SetFloat("moveY", moveDir.y);
            animator.SetFloat("lastMoveX", moveDir.x);
            animator.SetFloat("lastMoveY", moveDir.y);
        }
    }
}
