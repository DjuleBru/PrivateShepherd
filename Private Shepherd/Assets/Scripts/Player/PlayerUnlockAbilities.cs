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


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if(ES3.Load(barkLevelUnlock.levelName + "_completed", false)) {
            StartCoroutine(UnlockBark());
        };
        if (ES3.Load(growlLevelUnlock.levelName + "_completed", false)) {
            StartCoroutine(UnlockRun());
        };
        if (ES3.Load(runLevelUnlock.levelName + "_completed", false)) {
            StartCoroutine(UnlockSneak());
        };
        if (ES3.Load(sneakLevelUnlock.levelName + "_completed", false)) {
            StartCoroutine(UnlockGrowl());
        };
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
