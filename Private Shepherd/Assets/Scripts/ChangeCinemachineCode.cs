using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCinemachineCode : MonoBehaviour
{
    public static ChangeCinemachineCode Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake() {
        Instance = this;
    }

    public void ChangeCinemachineTarget(Transform target) {
        cinemachineVirtualCamera.LookAt = target;
        cinemachineVirtualCamera.Follow = target;
    }

    public float GetCinemachineOrthoSize() {
        return cinemachineVirtualCamera.m_Lens.OrthographicSize;
    }

    public void ChangeCinemachineOrthoSize(float orthoSize) {
        cinemachineVirtualCamera.m_Lens.OrthographicSize = orthoSize;
    }
}
