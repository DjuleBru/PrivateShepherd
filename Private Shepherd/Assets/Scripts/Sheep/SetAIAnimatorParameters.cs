using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAIAnimatorParameters : MonoBehaviour {
    [SerializeField] private Animator animator;
    [SerializeField] private AIMovement AIMovement;

    Vector2 moveDir;

    private void FixedUpdate() {

        if (AIMovement.GetReachedEndOfPath()) {
            animator.SetBool("isMoving", false);
        } else {
            moveDir = AIMovement.GetMoveDirNormalized();
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", moveDir.x);
            animator.SetFloat("moveY", moveDir.y);
            animator.SetFloat("lastMoveX", moveDir.x);
            animator.SetFloat("lastMoveY", moveDir.y);
        }
    }
}
