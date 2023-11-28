using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIntroCutScene : LevelCutScene {

    public static LevelIntroCutScene Instance { get; private set; }

    [SerializeField] Transform cameraFollowTransform;
    [SerializeField] Transform[] cameraPositions;
    private int cameraPositionInt;

    [SerializeField] float NPCZoom;
    [SerializeField] float initialZoom;
    private bool NPCIsTalking;
    private bool cameraLocked;
    private Transform cameraPosition;

    float initialOrthoSize;

    protected void Awake() {
        Instance = this;
    }


    protected override void Start() {
        base.Start();
        Invoke("EnterCutScene",.1f);
        StartCoroutine(IntroCutScene());
        initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();
    }

    protected override void Update() {
        base.Update();
        if (cameraLocked) {
            cameraFollowTransform.position = cameraPosition.position;
        }
    }

    private IEnumerator IntroCutScene() {
        LevelIntroNPC.Instance.SetInputActive(false);
        yield return new WaitForSeconds(.5f);

        // Initialize camera transform and dezoom

        cameraFollowTransform.transform.position = Player.Instance.transform.position;
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(cameraFollowTransform);
        ChangeCinemachineCode.Instance.SetCameraPriority(20);

        // Smooth camera movement towards NPC
        Vector3 destination = LevelIntroNPC.Instance.transform.position;
        float treshold = .2f;
        float cameraSpeed = .2f;
        Vector3 directionNormalized = (destination - cameraFollowTransform.position).normalized;

        while ((cameraFollowTransform.position - destination).magnitude > treshold) {
            cameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        cameraFollowTransform.position = destination;

        // Camera zoom
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(initialZoom);
        yield return new WaitForSeconds(1f);

        // NPC Starts talking
        NPCIsTalking = true;
        LevelIntroNPC.Instance.StartTalking();
        while (NPCIsTalking) yield return null;

        // Back camera to player
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(Player.Instance.transform);
        ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(initialOrthoSize);
        ExitCutScene();
        ChangeCinemachineCode.Instance.SetCameraPriority(0);

        yield return null;
    }

    public void MoveCameraToNextPosition(float cameraSpeed, float cameraZoom) {
        LevelIntroNPC.Instance.SetInputActive(false);
        Transform targetPosition = cameraPositions[cameraPositionInt];
        StartCoroutine(MoveCameraToPositionCoroutine(targetPosition, cameraSpeed, cameraZoom));
        cameraPositionInt++;
    }

    public void LockCameraToNextPosition(float cameraSpeed, float cameraZoom) {
        LevelIntroNPC.Instance.SetInputActive(false);
        Transform targetPosition = cameraPositions[cameraPositionInt];
        StartCoroutine(LockCameraToPositionCoroutine(targetPosition, cameraSpeed, cameraZoom));
        cameraPositionInt++;
    }

    private IEnumerator MoveCameraToPositionCoroutine(Transform targetPosition, float cameraSpeed, float cameraZoom) {
        float initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();

        // Initialize camera transform and zoom
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(cameraFollowTransform);
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(cameraZoom);

        // Smooth camera movement towards target
        Vector3 destination = targetPosition.position;
        float treshold = .2f;
        Vector3 directionNormalized = (destination - cameraFollowTransform.position).normalized;

        while ((cameraFollowTransform.position - destination).magnitude > treshold) {
            cameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        cameraFollowTransform.position = destination;

        LevelIntroNPC.Instance.SetInputActive(true);
    }
    private IEnumerator LockCameraToPositionCoroutine(Transform targetPosition, float cameraSpeed, float cameraZoom) {
        float initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();

        // Initialize camera transform and zoom
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(cameraFollowTransform);
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(cameraZoom);

        // Smooth camera movement towards target
        Vector3 destination = targetPosition.position;
        float treshold = .2f;
        Vector3 directionNormalized = (destination - cameraFollowTransform.position).normalized;

        while ((cameraFollowTransform.position - destination).magnitude > treshold) {
            destination = targetPosition.position;
            directionNormalized = (destination - cameraFollowTransform.position).normalized;
            cameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        cameraFollowTransform.position = destination;
        cameraLocked = true;
        cameraPosition = targetPosition;

        LevelIntroNPC.Instance.SetInputActive(true);
    }


    public void SetCameraToNPC() { 
        // Initialize camera transform and dezoom
        cameraFollowTransform.transform.position = LevelIntroNPC.Instance.transform.position;
        ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(NPCZoom);
    }

    public void SetNPCIsTalking(bool isTalking) {
        NPCIsTalking = isTalking;
    }

    public override void SkipCutScene() {
        base.SkipCutScene();
        LevelIntroNPC.Instance.StopTalking();
    }

    public bool GetCameraLocked() {
        return cameraLocked;
    }

    public void SetCameraLocked(bool locked) {
        cameraLocked = locked;
    }
}
