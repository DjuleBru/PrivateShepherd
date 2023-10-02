using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBark : MonoBehaviour
{
    public static PlayerBark Instance { get; private set; }
    public event EventHandler OnPlayerBark;

    private bool barkUnlocked = true;
    private float barkCoolDownTimer = 5f;
    private float barkCoolDown = .5f;

    private void Awake() {
        Instance = this;
    }

}
