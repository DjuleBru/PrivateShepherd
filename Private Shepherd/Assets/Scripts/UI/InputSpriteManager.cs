using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSpriteManager : MonoBehaviour
{

    public static InputSpriteManager Instance { get; private set; }

    [SerializeField] InputSpritesSO gamePadSprites;
    [SerializeField] InputSpritesSO keyBoardSprites;

    private Sprite moveControl;
    private Sprite barkControl;
    private Sprite runControl;
    private Sprite growlControl;
    private Sprite exitSprite;

    private Vector3 moveSpriteScale;
    private Vector3 runSpriteScale;
    private Vector3 exitSpriteScale;
    private Vector3 growlSpriteScale;
    private Vector3 barkSpriteScale;

    private PlayerInput playerInput;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        playerInput = GetComponent<PlayerInput>();


        playerInput.onControlsChanged += PlayerInput_onControlsChanged;
        playerInput.onActionTriggered += PlayerInput_onActionTriggered;

        moveControl = gamePadSprites.moveControl;
        barkControl = gamePadSprites.barkControl;
        runControl = gamePadSprites.runControl;
        growlControl = gamePadSprites.growlControl;
        exitSprite = gamePadSprites.exitSprite;

        moveSpriteScale = Vector3.one;
        runSpriteScale = Vector3.one;
        exitSpriteScale = Vector3.one;
        growlSpriteScale = Vector3.one;
        barkSpriteScale = Vector3.one;
    }

    private void PlayerInput_onActionTriggered(InputAction.CallbackContext obj) {
        if (playerInput.currentControlScheme == "GamePad") {
            moveControl = gamePadSprites.moveControl;
            barkControl = gamePadSprites.barkControl;
            runControl = gamePadSprites.runControl;
            growlControl = gamePadSprites.growlControl;
            exitSprite = gamePadSprites.exitSprite;

            moveSpriteScale = Vector3.one;
            runSpriteScale = Vector3.one;
            exitSpriteScale = Vector3.one;
            growlSpriteScale = Vector3.one;
            barkSpriteScale = Vector3.one;
        }

        else if (playerInput.currentControlScheme == "KeyBoard") {
            moveControl = keyBoardSprites.moveControl;
            barkControl = (keyBoardSprites.barkControl);
            runControl = (keyBoardSprites.runControl);
            growlControl = (keyBoardSprites.growlControl);
            exitSprite = (keyBoardSprites.exitSprite);

            moveSpriteScale = Vector3.one;
            runSpriteScale = Vector3.one;
            exitSpriteScale = Vector3.one;
            growlSpriteScale = Vector3.one;
            barkSpriteScale = new Vector3(1f, .5f, 1f);
        }
    }

    public Sprite GetMoveSprite() {
        return moveControl;
    }

    public Sprite GetBarkSprite() {
        return barkControl;
    }

    public Sprite GetRunSprite() {
        return runControl;
    }

    public Sprite GetGrowlSprite() {
        return growlControl;
    }

    public Sprite GetExitSprite() {
        return exitSprite;
    }

    public Vector3 GetMoveSpriteScale() {
        return moveSpriteScale;
    }

    public Vector3 GetBarkSpriteScale() {
        return barkSpriteScale;
    }

    public Vector3 GetRunSpriteScale() {
        return runSpriteScale;
    }

    public Vector3 GetExitSpriteScale() {
        return exitSpriteScale;
    }

    public Vector3 GetGrowlSpriteScale() {
        return growlSpriteScale;
    }

    private void PlayerInput_onControlsChanged(PlayerInput obj) {

        if (playerInput.currentControlScheme == "GamePad") {
            moveControl = gamePadSprites.moveControl;
            barkControl = gamePadSprites.barkControl;
            runControl = gamePadSprites.runControl;
            growlControl = gamePadSprites.growlControl;
            exitSprite = gamePadSprites.exitSprite;

            moveSpriteScale = Vector3.one;
            runSpriteScale = Vector3.one;
            exitSpriteScale = Vector3.one;
            growlSpriteScale = Vector3.one;
            barkSpriteScale = Vector3.one;
        }

        else if (playerInput.currentControlScheme == "KeyBoard") {
            moveControl = keyBoardSprites.moveControl;
            barkControl =(keyBoardSprites.barkControl);
            runControl =(keyBoardSprites.runControl);
            growlControl =(keyBoardSprites.growlControl);
            exitSprite =(keyBoardSprites.exitSprite);

            moveSpriteScale = Vector3.one;
            runSpriteScale = Vector3.one;
            exitSpriteScale = Vector3.one;
            growlSpriteScale = Vector3.one;
            barkSpriteScale = new Vector3(1f, .5f, 1f);
        }
            

    }

}
