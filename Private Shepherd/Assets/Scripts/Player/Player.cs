using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private int bones;
    private int defaultBones = 0;

    public event EventHandler OnBonesChanged;

    [SerializeField] private bool androidPort;

    private void Awake() {
        Instance = this;

        bones = ES3.Load("playerBones", defaultBones);
    }

    private void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Update() {
        Debug.Log(1.0f / Time.deltaTime);
    }

    public int GetPlayerBones() {
        return bones;
    }

    public void SpendPlayerBones(int bonePrice) {
        bones -= bonePrice;
        OnBonesChanged?.Invoke(this, EventArgs.Empty);
        ES3.Save("playerBones", bones);
    }

    public void GivePlayerBones(int boneReward) {
        bones += boneReward;
        OnBonesChanged?.Invoke(this, EventArgs.Empty);
        ES3.Save("playerBones", bones);
    }

    public void SetPlayerPosition(Vector3 position) {
        transform.position = position;
    }

    public bool GetAndroidPort() {
        return androidPort;
    }

}
