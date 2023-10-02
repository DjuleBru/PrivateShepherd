using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementJoystick : MonoBehaviour {

    private Vector3 lastMoveDir;
    [SerializeField] private Joystick joyStick;

    Vector2 moveInput;
    Vector3 moveDir = new Vector3(0, 1, 0);

    bool isMoving;

    float animatorX;
    float animatorY;

    [SerializeField] Animator animator;

    void Start() {

    }

    void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        moveInput.x = joyStick.Horizontal;
        moveInput.y = joyStick.Vertical;

        if (moveInput == Vector2.zero) {
            isMoving = false;
        } else {
            isMoving = true;
        }

        SetLastMoveDir();
        SetAnimatorParameters();
        Move();
    }

    private void SetLastMoveDir() {

        if (moveInput == Vector2.zero && moveDir != Vector3.zero) {
            if (moveDir.x != 0 && moveDir.y == 0) {
                lastMoveDir.x = moveDir.x;
            }
            if (moveDir.y != 0 && moveDir.x == 0) {
                lastMoveDir.y = moveDir.y;
            }
            if (moveDir.y != 0 && moveDir.x != 0) {
                lastMoveDir = moveDir;
            }
        }
    }

    private void SetAnimatorParameters() {
        animatorX = moveInput.x;
        animatorY = moveInput.y;

        if (moveInput.x != 0 && moveInput.y == 0) {
            animatorY = lastMoveDir.y;
        }
        if (moveInput.x == 0 && moveInput.y != 0) {
            animatorX = lastMoveDir.x;
        }

        animator.SetFloat("lastMoveX", lastMoveDir.x);
        animator.SetFloat("lastMoveY", lastMoveDir.y);
        animator.SetFloat("moveX", animatorX);
        animator.SetFloat("moveY", animatorY);
        animator.SetBool("isMoving", isMoving);
    }

    private void Move() {
        moveDir = new Vector3(moveInput.x, moveInput.y, 0);
        Vector3 moveDirNormalized = moveDir.normalized;

        float moveSpeed = 3f;
        transform.position += moveDirNormalized * Time.deltaTime * moveSpeed;
    }

    private bool GetIsMoving() {
        return isMoving;
    }

    private Vector3 GetMoveDir() {
        return moveDir;
    }

}
