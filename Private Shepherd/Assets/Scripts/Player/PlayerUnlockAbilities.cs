using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerUnlockAbilities : MonoBehaviour
{

    public static PlayerUnlockAbilities Instance { get; private set; }

    [SerializeField] LevelSO barkLevelUnlock;
    [SerializeField] LevelSO growlLevelUnlock;
    [SerializeField] LevelSO runLevelUnlock;
    [SerializeField] LevelSO sneakLevelUnlock;

    public event EventHandler OnBarkUnlocked;
    public event EventHandler OnGrowlUnlocked;
    public event EventHandler OnRunUnlocked;
    public event EventHandler OnSneakUnlocked;

    private bool barkUnlocked;
    private bool growlUnlocked;
    private bool runUnlocked;
    private bool sneakUnlocked;

    private void Awake() {
        Instance = this;
    }

    private void Start() {

        barkUnlocked = ES3.Load("barkUnlocked", false);
        growlUnlocked = ES3.Load("growlUnlocked", false);
        sneakUnlocked = ES3.Load("sneakUnlocked", false);
        runUnlocked = ES3.Load("runUnlocked", false);

        if(!barkUnlocked) {
            if (ES3.Load(barkLevelUnlock.levelName + "_completed", false)) {
                StartCoroutine(UnlockBark());
            };
        }

        if(growlUnlocked) {
            if (ES3.Load(growlLevelUnlock.levelName + "_completed", false)) {
                StartCoroutine(UnlockRun());
            };
        }

        if(!sneakUnlocked) {
            if (ES3.Load(sneakLevelUnlock.levelName + "_completed", false)) {
                StartCoroutine(UnlockSneak());
            };
        }

        if(!runUnlocked) {
            if (ES3.Load(runLevelUnlock.levelName + "_completed", false)) {
                StartCoroutine(UnlockGrowl());
            };
        }

    }

    private IEnumerator UnlockBark() {
        ES3.Save("barkUnlocked", true);
        Player.Instance.GetComponent<PlayerBark>().SetBarkUnlocked(true);
        OnBarkUnlocked?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private IEnumerator UnlockRun() {
        ES3.Save("runUnlocked", true);
        Player.Instance.GetComponent<PlayerRun>().SetRunUnlocked(true);
        OnRunUnlocked?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private IEnumerator UnlockSneak() {
        ES3.Save("sneakUnlocked", true);
        Player.Instance.GetComponent<PlayerSneak>().SetSneakUnlocked(true);
        OnSneakUnlocked?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

    private IEnumerator UnlockGrowl() {
        ES3.Save("growlUnlocked", true);
        Player.Instance.GetComponent<PlayerGrowl>().SetGrowlUnlocked(true);
        OnGrowlUnlocked?.Invoke(this, EventArgs.Empty);
        yield return null;
    }

}
