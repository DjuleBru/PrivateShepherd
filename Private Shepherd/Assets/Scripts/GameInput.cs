using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public event EventHandler OnBarkPerformed;
    public event EventHandler OnBarkReleased;
    public event EventHandler OnBarkPressed;
    public event EventHandler OnGrowlPerformed;
    public event EventHandler OnGrowlReleased;
    public event EventHandler OnRunPerformed;
    public event EventHandler OnRunReleased;
    public event EventHandler OnExitPerformed;

    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

    }

    

    public Vector2 GetMovementVector() {
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        return moveInput;
    }

    private void OnEnable() {
        playerInputActions.Player.Enable();
    }

    private void OnDisable() {
        playerInputActions.Player.Disable();
    }

    private void OnDestroy() {
        playerInputActions.Player.Bark.performed -= Bark_performed;
    }
}
