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

    public void SetCameraPriority(int priority) {
        cinemachineVirtualCamera.Priority = priority;
    }

    public float GetCinemachineOrthoSize() {
        return cinemachineVirtualCamera.m_Lens.OrthographicSize;
    }

    public void ChangeCinemachineOrthoSize(float orthoSize) {
        cinemachineVirtualCamera.m_Lens.OrthographicSize = orthoSize;
    }

    public void SmoothCinemachineZoom(float targetOrthoSize) {
        StartCoroutine(SmoothCinemachineZoomCoroutine(targetOrthoSize));
    }

    public IEnumerator SmoothCinemachineZoomCoroutine(float targetOrthoSize) {
        float initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();
        float treshold = .1f;

        float dynamicOrthoSize = initialOrthoSize;
        float step = (targetOrthoSize - initialOrthoSize) / 20f;

        while (Mathf.Abs(dynamicOrthoSize - targetOrthoSize) > treshold) {
            dynamicOrthoSize += step;

            ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(dynamicOrthoSize);
            yield return null;
        }
        yield return null;
    }
}
