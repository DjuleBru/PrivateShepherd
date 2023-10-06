using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public event EventHandler OnBarkPerformed;

    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Bark.performed += Bark_performed;
    }

    private void Bark_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnBarkPerformed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector() {
        Vector2 moveInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        return moveInput;
    }
}
