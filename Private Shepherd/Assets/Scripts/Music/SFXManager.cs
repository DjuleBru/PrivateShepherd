using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    [SerializeField] private SFXSO SFXSO;
    private Camera mainCamera;

    private float volume;

    AudioSource growlAudioSource;
    AudioSource moveAudioSource;
    AudioSource tiredAudioSource;

    private bool isMoving;
    private bool sneaking;

    private float defaultSFXVolume = .4f;
    private float halfDefaultSFXVolume;
    private float volumeTick;

    [SerializeField] private bool mainMenu;

    private void Awake() {
        Instance = this;

        halfDefaultSFXVolume = defaultSFXVolume / 2f;
        volume = ES3.Load("sfxVolume", halfDefaultSFXVolume);

        volumeTick = defaultSFXVolume / 20f;
    }

    private void Start() {
        mainCamera = Camera.main;

        WolfAI.OnAnyWolfFlee += WolfAI_OnAnyWolfFlee;
        WolfAI.OnAnySheepEaten += WolfAI_OnAnySheepEaten;
        WolfAI.OnAnySheepBite += WolfAI_OnAnySheepBite;

        Sheep.OnAnySheepEnterScoreZone += Sheep_OnAnySheepEnterScoreZone;
        SheepMovement.OnAnySheepFlee += SheepMovement_OnAnySheepFlee;
        SheepMovement.OnAnySheepFleeAggregate += SheepMovement_OnAnySheepFleeAggregate;
        SheepMovement.OnAnyGoatRam += SheepMovement_OnAnyGoatRam;
        SheepMovement.OnAnyGoatRamWarning += SheepMovement_OnAnyGoatRamWarning;

        Tentacle.OnAnySheepBite += Tentacle_OnAnySheepBite;
        Tentacle.OnAnySheepEaten += Tentacle_OnAnySheepEaten;
        Tentacle.OnAnyTentacleEmerge += Tentacle_OnAnyTentacleEmerge;

        ButtonScript.OnAnyButtonClick += ButtonScript_OnAnyButtonClick;
        ButtonScript.OnAnyButtonSelected += ButtonScript_OnAnyButtonSelected;

        if (mainMenu) {
            return;
        }

        PlayerGrowl.Instance.OnPlayerGrowl += PlayerGrowl_OnPlayerGrowl;
        PlayerGrowl.Instance.OnPlayerGrowlReleased += PlayerGrowl_OnPlayerGrowlReleased;
        PlayerBark.Instance.OnPlayerBark += PlayerBark_OnPlayerBark;
        PlayerRun.Instance.onPlayerTired += PlayerRun_onPlayerTired;
        PlayerRun.Instance.onPlayerRecovered += PlayerRun_onPlayerRecovered;
        PlayerRun.Instance.OnPlayerRun += PlayerRun_OnPlayerRun;
        PlayerRun.Instance.OnPlayerRunStop += PlayerRun_OnPlayerRunStop;
        PlayerSneak.Instance.OnPlayerSneakReleased += PlayerSneak_OnPlayerSneakReleased;
        PlayerSneak.Instance.OnPlayerSneakStarted += PlayerSneak_OnPlayerSneakStarted;

        moveAudioSource = Player.Instance.GetComponent<AudioSource>();
        tiredAudioSource = PlayerRun.Instance.GetAudioSource();
    }


    private void Update() {
        if(sneaking | mainMenu) {
            return;
        }

        if(PlayerMovement.Instance.GetIsMoving() &!isMoving) {
            moveAudioSource.clip = SFXSO.playerMove;
            moveAudioSource.volume = GetVolume() * 2f;
            moveAudioSource.Play();
            isMoving = true;
        } 

        if(!PlayerMovement.Instance.GetIsMoving()) {
            moveAudioSource.Stop();
            isMoving = false;
        }
    }

    public float GetVolume() {
        return volume;
    }

    public void IncreaseVolume() {
        if (volume <= defaultSFXVolume) {
            volume += volumeTick;
        }
    }
    public void DecreaseVolume() {
        if (volume >= 0f) {
            volume -= volumeTick;
        }
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) { 
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    private void WolfAI_OnAnySheepBite(object sender, System.EventArgs e) {
        float volume = GetVolume() * 3f;
        WolfAI wolfAI = (WolfAI)sender;
        PlaySound(SFXSO.wolfBites, wolfAI.transform.position, volume);
    }

    private void WolfAI_OnAnySheepEaten(object sender, System.EventArgs e) {
        float volume = GetVolume() * 2f;
        WolfAI wolfAI = (WolfAI)sender;
        PlaySound(SFXSO.wolfEats, wolfAI.transform.position, volume);
    }

    private void WolfAI_OnAnyWolfFlee(object sender, System.EventArgs e) {
        float volume = GetVolume() * 2f;
        WolfAI wolfAI = (WolfAI)sender;
        PlaySound(SFXSO.wolfFlee, wolfAI.transform.position, volume);
    }

    private void Sheep_OnAnySheepEnterScoreZone(object sender, System.EventArgs e) {
        float volume = GetVolume() * 2f;
        Sheep sheep = (Sheep)sender;
        PlaySound(SFXSO.sheepEnterScoreZone, sheep.transform.position, volume);
    }

    private void SheepMovement_OnAnySheepFlee(object sender, System.EventArgs e) {
        SheepMovement sheep = (SheepMovement)sender;
        float volume = GetVolume() * .1f;
        PlaySound(SFXSO.sheepBaa, sheep.transform.position);
    }

    private void SheepMovement_OnAnySheepFleeAggregate(object sender, System.EventArgs e) {
        SheepMovement sheep = (SheepMovement)sender;
        float volume = GetVolume() * 3f;
        if (Random.Range(0,100) > 15) {
            PlaySound(SFXSO.sheepBaaAggregate, sheep.transform.position, volume);
        }
    }

    private void SheepMovement_OnAnyGoatRamWarning(object sender, System.EventArgs e) {
        SheepMovement sheep = (SheepMovement)sender;
        PlaySound(SFXSO.goatRamWarning, sheep.transform.position);
    }

    private void SheepMovement_OnAnyGoatRam(object sender, System.EventArgs e) {
        SheepMovement sheep = (SheepMovement)sender;
        PlaySound(SFXSO.goatRam, sheep.transform.position);
    }

    private void Tentacle_OnAnyTentacleEmerge(object sender, System.EventArgs e) {
        float volume = GetVolume() * 4f;
        Tentacle tentacle = (Tentacle)sender;
        PlaySound(SFXSO.tentacleEmerge, tentacle.transform.position);
    }

    private void Tentacle_OnAnySheepEaten(object sender, System.EventArgs e) {
        float volume = GetVolume() * 4f;
        Tentacle tentacle = (Tentacle)sender;
        PlaySound(SFXSO.tentacleEats, tentacle.transform.position, volume);
    }

    private void Tentacle_OnAnySheepBite(object sender, System.EventArgs e) {
        float volume = GetVolume() * 4f;
        Tentacle tentacle = (Tentacle)sender;
        PlaySound(SFXSO.tentacleHits, tentacle.transform.position, volume);
    }

    private void ButtonScript_OnAnyButtonSelected(object sender, System.EventArgs e) {
        bool paused = false;

        float volume = GetVolume() * .5f;
        if (Time.timeScale == 0) {
            paused = true;
            Time.timeScale = 1.0f;
        }
        
        if(mainCamera == null) {
            mainCamera = Camera.main;
        }
        PlaySound(SFXSO.UIButtonHover, mainCamera.transform.position, volume);

        if(paused) {
            Time.timeScale = 0f;
        }
    }

    private void ButtonScript_OnAnyButtonClick(object sender, System.EventArgs e) {
        PlaySound(SFXSO.UIButtonClick, mainCamera.transform.position, volume);
    }


    private void PlayerBark_OnPlayerBark(object sender, System.EventArgs e) {
        float volume = GetVolume() * 2f;
        PlaySound(SFXSO.playerBark, mainCamera.transform.position, volume);
    }

    private void PlayerGrowl_OnPlayerGrowlReleased(object sender, System.EventArgs e) {
        growlAudioSource.Stop();
    }

    private void PlayerGrowl_OnPlayerGrowl(object sender, System.EventArgs e) {
        growlAudioSource = PlayerGrowl.Instance.GetGrowlAudioSource();
        growlAudioSource.clip = SFXSO.playerGrowl[Random.Range(0, SFXSO.playerGrowl.Length)];
        growlAudioSource.volume = volume;
        growlAudioSource.Play();
    }

    private void PlayerRun_onPlayerTired(object sender, System.EventArgs e) {
        tiredAudioSource.clip = SFXSO.playerTired;
        float volume = GetVolume() * .8f;
        tiredAudioSource.volume = volume;
        tiredAudioSource.Play();
    }

    private void PlayerRun_OnPlayerRunStop(object sender, System.EventArgs e) {
        moveAudioSource.Stop();
        isMoving = false;
    }

    private void PlayerRun_OnPlayerRun(object sender, System.EventArgs e) {
        float volume = GetVolume() * 2f;
        moveAudioSource.clip = SFXSO.playerRun;
        moveAudioSource.volume = volume;
        moveAudioSource.Play();
    }

    private void PlayerRun_onPlayerRecovered(object sender, System.EventArgs e) {
        tiredAudioSource.Stop();
    }


    private void PlayerSneak_OnPlayerSneakStarted(object sender, System.EventArgs e) {
        sneaking = true;
        moveAudioSource.Stop();
        moveAudioSource.clip = SFXSO.playerSneak;
        moveAudioSource.volume = volume;
        moveAudioSource.Play();
    }

    private void PlayerSneak_OnPlayerSneakReleased(object sender, System.EventArgs e) {
        sneaking = false;
        isMoving = false;   
        moveAudioSource.Stop();
    }

}
