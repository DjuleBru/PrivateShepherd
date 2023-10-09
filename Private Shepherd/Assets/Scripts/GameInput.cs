using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public event EventHandler OnBarkPerformed;
    public event EventHandler OnBarkReleased;
    public event EventHandler OnGrowlPerformed;
    public event EventHandler OnGrowlReleased;

    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Bark.performed += Bark_performed;
        playerInputActions.Player.Bark.canceled += Bark_released;

        playerInputActions.Player.Growl.performed += Growl_performed;
        playerInputActions.Player.Growl.canceled += Growl_canceled;
    }

    private void Growl_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnGrowlReleased?.Invoke(this, EventArgs.Empty);
    }

    private void Growl_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnGrowlPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Bark_released(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnBarkReleased?.Invoke(this, EventArgs.Empty);
    }

    private void Bark_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnBarkPerformed?.Invoke(this, EventArgs.Empty);
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
