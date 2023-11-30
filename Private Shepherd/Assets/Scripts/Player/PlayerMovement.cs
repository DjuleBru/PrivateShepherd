using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    private Rigidbody2D rb;
    private Vector3 lastMoveDir;
    Vector2 moveInput;
    Vector3 moveDir = new Vector3(0, 1, 0);

    bool isMoving;
    bool canMove;

    float animatorX;
    float animatorY;
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;

    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        canMove = true;
    }

    void Update() {
        if (canMove) {
            HandleMovementInput();
        } else {
            isMoving = false;
            moveInput = Vector2.zero;
        }
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void HandleMovementInput() {
        moveInput = GameInput.Instance.GetMovementVector();

        if (moveInput == Vector2.zero) {
            isMoving = false;
        } else {
            isMoving = true;
        }

    }

    private void HandleMovement() {
        SetLastMoveDir();

        SetAnimatorParameters();
        if (canMove) {
            Move();
        }

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

        rb.velocity = moveDirNormalized * moveSpeed * Time.fixedDeltaTime;
    }

    private bool GetIsMoving() {
        return isMoving;
    }

    private Vector3 GetMoveDir() {
        return moveDir;
    }

    public void SetCanMove(bool canMove) {
        this.canMove = canMove;
        rb.velocity = Vector3.zero;
    }

    public void Rammed(Vector2 force) {
        canMove = false;
        rb.velocity = Vector3.zero;
        Debug.Log(force);
        rb.AddForce(force);

        StartCoroutine(RammedCoroutine());
    }

    private IEnumerator RammedCoroutine() {
        yield return new WaitForSeconds(1f);
        canMove = true;
        yield return null;
    }

    public void SetMoveSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }

}
