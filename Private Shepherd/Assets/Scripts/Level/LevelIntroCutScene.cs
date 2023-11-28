using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIntroCutScene : LevelCutScene {

    public static LevelIntroCutScene Instance { get; private set; }

    [SerializeField] Transform cameraFollowTransform;
    [SerializeField] Transform[] cameraPositions;
    private int cameraPositionInt;
    [SerializeField] LevelIntroNPC NPC;

    [SerializeField] float NPCZoom;
    [SerializeField] float CameraIntroDezoom;
    private bool NPCIsTalking;

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

    private IEnumerator IntroCutScene() {
        NPC.SetInputActive(false);
        yield return new WaitForSeconds(.5f);

        // Initialize camera transform and dezoom

        cameraFollowTransform.transform.position = Player.Instance.transform.position;
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(cameraFollowTransform);
        ChangeCinemachineCode.Instance.SetCameraPriority(20);

        // Smooth camera movement towards NPC
        Vector3 destination = NPC.transform.position;
        float treshold = .2f;
        float cameraSpeed = .2f;
        Vector3 directionNormalized = (destination - cameraFollowTransform.position).normalized;

        while ((cameraFollowTransform.position - destination).magnitude > treshold) {
            cameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        cameraFollowTransform.position = destination;

        // Camera zoom
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(NPCZoom);
        yield return new WaitForSeconds(1f);

        // NPC Starts talking
        NPCIsTalking = true;
        NPC.StartTalking();
        while (NPCIsTalking) yield return null;

        // Back camera to player
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(Player.Instance.transform);
        ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(initialOrthoSize);
        ExitCutScene();
        ChangeCinemachineCode.Instance.SetCameraPriority(0);

        yield return null;
    }

    public void MoveCameraToNextPosition(float cameraSpeed) {
        NPC.SetInputActive(false);
        Transform targetPosition = cameraPositions[cameraPositionInt];
        StartCoroutine(MoveCameraToPositionCoroutine(targetPosition, cameraSpeed));
        cameraPositionInt++;
    }

    private IEnumerator MoveCameraToPositionCoroutine(Transform targetPosition, float cameraSpeed) {
        float initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();

        // Initialize camera transform and dezoom
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(cameraFollowTransform);
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(CameraIntroDezoom);
        yield return new WaitForSeconds(.5f);

        // Smooth camera movement towards target
        Vector3 destination = targetPosition.position;
        float treshold = .2f;
        Vector3 directionNormalized = (destination - cameraFollowTransform.position).normalized;

        while ((cameraFollowTransform.position - destination).magnitude > treshold) {
            cameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        cameraFollowTransform.position = destination;

        NPC.SetInputActive(true);
    }

    public void SetCameraToNPC() { 
        // Initialize camera transform and dezoom
        cameraFollowTransform.transform.position = NPC.transform.position;
        ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(NPCZoom);
    }

    public void SetNPCIsTalking(bool isTalking) {
        NPCIsTalking = isTalking;
    }

    public override void SkipCutScene() {
        base.SkipCutScene();
        NPC.StopTalking();
    }
}
