using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Vector3 lastMoveDir;
    Vector2 moveInput;
    Vector3 moveDir = new Vector3(0, 1, 0);

    bool isMoving;

    float animatorX;
    float animatorY;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Animator animator;

    void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.Z)) {
            moveInput.y = +1;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveInput.y = -1;
        }
        if (Input.GetKey(KeyCode.Q)) {
            moveInput.x = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveInput.x = +1;
        }

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

        transform.position += moveDirNormalized * Time.deltaTime * moveSpeed;
    }

    private bool GetIsMoving() {
        return isMoving;
    }

    private Vector3 GetMoveDir() {
        return moveDir;
    }

}
