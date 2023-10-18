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

    private void Awake() {
        Instance = this; 
    }

    private void Start() {
        bones = ES3.Load("playerBones", defaultBones);
    }

    public int GetPlayerBones() {
        return bones;
    }

    public void SpendPlayerBones(int bonePrice) {
        bones -= bonePrice;
        ES3.Save("playerBones", bones);
    }

    public void GivePlayerBones(int boneReward) {
        bones += boneReward;
        ES3.Save("playerBones", bones);
    }

}
