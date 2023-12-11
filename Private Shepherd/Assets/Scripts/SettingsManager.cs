using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager Instance { get; private set; }

    private float musicVolume;
    private float musicDisplayVolume;
    private float sfxVolume;
    private float sfxDisplayVolume;
    private bool sheepNames;
    private bool blood;

    [SerializeField] Image musicBar;
    [SerializeField] Image sfxBar;

    [SerializeField] GameObject sheepNamesToggleImage;
    [SerializeField] GameObject bloodToggleImage;

    public event EventHandler OnSheepNamesToggled;
    public event EventHandler OnBloodToggled;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        musicVolume = ES3.Load("musicVolume", .1f);
        sfxVolume = ES3.Load("sfxVolume", .2f);
        sheepNames = ES3.Load("sheepNames",true);
        blood = ES3.Load("blood", true);

        sheepNamesToggleImage.SetActive(sheepNames);
        bloodToggleImage.SetActive(blood);

        musicDisplayVolume = musicVolume * 5f;
        musicBar.fillAmount = musicDisplayVolume;

        sfxDisplayVolume = sfxVolume * 2.5f;
        sfxBar.fillAmount = sfxDisplayVolume;
        Debug.Log(sfxVolume);
        Debug.Log(sfxDisplayVolume);
    }

    public void IncreaseMusicVolume() {
        MusicManager.Instance.IncreaseVolume();
        musicVolume = MusicManager.Instance.GetVolume();
        ES3.Save("musicVolume", musicVolume);

        musicDisplayVolume = musicVolume * 5f;
        musicBar.fillAmount = musicDisplayVolume;

    }

    public void DecreaseMusicVolume() {
        MusicManager.Instance.DecreaseVolume();
        musicVolume = MusicManager.Instance.GetVolume();
        ES3.Save("musicVolume", musicVolume);


        musicDisplayVolume = musicVolume * 5f;
        musicBar.fillAmount = musicDisplayVolume;
    }

    public void DecreaseSFXVolume() {
        SFXManager.Instance.DecreaseVolume();
        sfxVolume = SFXManager.Instance.GetVolume();
        ES3.Save("sfxVolume", sfxVolume);
        sfxBar.fillAmount = sfxVolume;


        sfxDisplayVolume = sfxVolume * 2.5f;
        sfxBar.fillAmount = sfxDisplayVolume;
        Debug.Log(sfxVolume);
        Debug.Log(sfxDisplayVolume);
    }

    public void IncreaseSFXVolume() {
        SFXManager.Instance.IncreaseVolume();
        sfxVolume = SFXManager.Instance.GetVolume();
        ES3.Save("sfxVolume", sfxVolume);
        sfxBar.fillAmount = sfxVolume;

        sfxDisplayVolume = sfxVolume * 2.5f;
        sfxBar.fillAmount = sfxDisplayVolume;

        Debug.Log(sfxVolume);
        Debug.Log(sfxDisplayVolume);
    }

    public void RefreshSoundBars() {
        musicBar.fillAmount = musicVolume;
        sfxBar.fillAmount = sfxVolume;
    }

    public void ToggleSheepNames() {
        sheepNames = !sheepNames;
        sheepNamesToggleImage.SetActive(sheepNames);
        ES3.Save("sheepNames", sheepNames);
        OnSheepNamesToggled?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleBlood() {
        blood = !blood;
        bloodToggleImage.SetActive(blood);
        ES3.Save("blood", blood);
        OnBloodToggled?.Invoke(this, EventArgs.Empty);
    }

}
