using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    LevelSO levelSO;
    [SerializeField] bool mainMenu;
    [SerializeField] bool worldMap;
    [SerializeField] bool dangerInLevel;

    [SerializeField] WolfActivator[] wolfActivators;
    [SerializeField] List<Tentacle> tentacles = new List<Tentacle>();
    [SerializeField] GameObjectActivator[] gameObjectActivators;

    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip worldMapMusic;

    private AudioSource audioSource;
    AudioClip levelMusic;
    AudioClip dangerMusic;

    private float defaultVolume = .2f;
    private float volumeTick;
    private float volume;

    private bool dangerEmerged;
    private bool musicFadedOut;
    private bool musicFadedIn;

    private float musicFadeSpeed;

    private void Awake() {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        volume = ES3.Load("musicVolume", defaultVolume/2f);

        musicFadeSpeed = defaultVolume / 5f;
        volumeTick = defaultVolume / 20f;
    }

    private void Start() {
        if(!mainMenu & !worldMap) {
            levelSO = LevelManager.Instance.GetLevelSO();

            levelMusic = levelSO.levelMusic;
            audioSource.clip = levelMusic;
            if (dangerInLevel) {
                dangerMusic = levelSO.dangerMusic;

                if (wolfActivators.Length > 0) {
                    foreach (WolfActivator wolfActivator in wolfActivators) {
                        wolfActivator.OnWolfActivated += WolfActivator_OnWolfActivated;
                    }
                }
                if (gameObjectActivators.Length > 0) {
                    foreach (GameObjectActivator game in gameObjectActivators) {
                        game.OnGameObjectActivated += Game_OnGameObjectActivated;
                    }
                }
            }
        } else {
            if(worldMap) {
                levelMusic = worldMapMusic;
            }
            if(mainMenu) {
                levelMusic = mainMenuMusic;
            }
        }

        audioSource.clip = levelMusic;

        audioSource.volume = 0;
        audioSource.Play();
        StartCoroutine(FadeMusicIn());
    }


    public void IncreaseVolume() {
        if(volume <= defaultVolume) {
            volume += volumeTick;
        }
        audioSource.volume = volume;
    }

    public void DecreaseVolume() {
        if (volume >= 0f) {
            volume -= volumeTick;
        }
        audioSource.volume = volume;
    }

    public float GetVolume() {
        return volume;
    }

    private void WolfActivator_OnWolfActivated(object sender, System.EventArgs e) {
        if (!dangerEmerged) {
            StartCoroutine(FadeMusicOut());
            StartCoroutine(SwapMusicToDanger());
        }
    }

    private void Tentacle_OnTentacleEmerge(object sender, System.EventArgs e) {
        if (!dangerEmerged) {
            StartCoroutine(FadeMusicOut());
            StartCoroutine(SwapMusicToDanger());
        }
    }

    private void Game_OnGameObjectActivated(object sender, System.EventArgs e) {
        if (!dangerEmerged) {
            StartCoroutine(FadeMusicOut());
            StartCoroutine(SwapMusicToDanger());
        }
    }

    private IEnumerator SwapMusicToDanger() {
        while (!musicFadedOut) {
            yield return new WaitForEndOfFrame();
        }
        audioSource.clip = dangerMusic;
        audioSource.Play();
        StartCoroutine(FadeMusicIn());
        yield return null;
    }

    private IEnumerator FadeMusicOut() {
        while(audioSource.volume > 0f) {
            audioSource.volume -= musicFadeSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        musicFadedOut = true;
        yield return null;
    }

    private IEnumerator FadeMusicIn() {
        while (audioSource.volume <= volume) {
            audioSource.volume += musicFadeSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        musicFadedIn = true;
        yield return null;
    }

    public void AddTentacleToList(Tentacle tentacle) {
        tentacles.Add(tentacle);
        tentacle.OnTentacleEmerge += Tentacle_OnTentacleEmerge;
        musicFadeSpeed = 2f;
    }

}
