using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private SheepSO whiteSheepSO;
    private Sheep[] initialSheepsInLevel;
    [SerializeField] private LevelSO levelSO;

    private int platScoreTreshold;
    private int silverScoreTreshold;
    private int goldScoreTreshold;
    private int bronzeScoreTreshold;

    private int initialSheepNumber;
    private int realTimeSheepNumber;
    private int pennedSheepNumber = 0;
    private int whitePennedSheepNumber;
    private int goldPennedSheepNumber;
    private int blackPennedSheepNumber;
    private int bluePennedSheepNumber;
    private int greenPennedSheepNumber;
    private int redPennedSheepNumber;

    private float levelTimeLimit;
    private float levelTimer;
    private float levelRemainingTime;
    private int timeScore;

    private int playerScore;
    private int defaultHighScore = 0;
    private int highScore;
    private int playerBones;
    private int highestBones;
    private int defaultHighestBones;

    private int whiteSheepScore;
    private int blackSheepScore;
    private int goldSheepScore;
    private int blueSheepScore;
    private int redSheepScore;
    private int greenSheepScore;

    private bool levelComplete;
    private bool bronzeBone;
    private bool silverBone;
    private bool goldBone;
    private bool platBone;
    public event EventHandler<OnScoreUpdateEventArgs> OnScoreUpdate;
    public event EventHandler<OnLevelSucceededEventArgs> OnLevelSucceeded;
    public event EventHandler OnLevelFailed;

    public class OnScoreUpdateEventArgs : EventArgs {
        public int realTimeSheepNumber;
        public int pennedSheepNumber;
    }

    public class OnLevelSucceededEventArgs : EventArgs {
        public int initialSheepNumber;
        public int pennedSheepNumber;
        public float levelRemainingTime;
        public int playerScore;
        public int playerTrophies;
    }

    private void Awake() {
        Instance = this;

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;

        // Initialise sheeps
        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();
        initialSheepNumber = initialSheepsInLevel.Length;
        realTimeSheepNumber = initialSheepNumber;
        whiteSheepScore = levelSO.whiteSheepScore;
        blackSheepScore = levelSO.blackSheepScore;
        blueSheepScore = levelSO.blueSheepScore;
        redSheepScore = levelSO.redSheepScore;
        goldSheepScore = levelSO.goldSheepScore;
        greenSheepScore = levelSO.greenSheepScore;

        foreach (Sheep sheep in initialSheepsInLevel) {
            sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;
        }

        // Initialise score thresholds
        bronzeScoreTreshold = levelSO.bronzeScoreTreshold;   
        silverScoreTreshold = levelSO.silverScoreTreshold;
        goldScoreTreshold = levelSO.goldScoreTreshold;
        platScoreTreshold = levelSO.platScoreTreshold;
        timeScore = levelSO.timeScore;

        // Initialise timers
        levelTimeLimit = levelSO.levelTimeLimit;
        levelTimer = levelTimeLimit;

        playerBones = 0;
        playerScore = 0;
    }

    private void Start() {
        LoadPlayerScores();
    }

    private void Update() {
        levelTimer -= Time.deltaTime;

        if (levelTimer < 0 & !levelComplete) {
            DisablePlayer();
            levelComplete = true;
            OnLevelFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, Sheep.OnSheepEnterScoreZoneEventArgs e) {
        SortSheepPennedByType(sender as Sheep);
        pennedSheepNumber++;

        OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventArgs {
            realTimeSheepNumber = realTimeSheepNumber,
            pennedSheepNumber = pennedSheepNumber
        });

        if (pennedSheepNumber == realTimeSheepNumber & !levelComplete) {
            EndLevelSuccess();
        }
    }
    private void LevelSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
        realTimeSheepNumber --;

        OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventArgs {
            realTimeSheepNumber = realTimeSheepNumber,
            pennedSheepNumber = pennedSheepNumber
        });

        if (realTimeSheepNumber == 0 & !levelComplete) {
            levelComplete = true;
            OnLevelFailed?.Invoke(this, EventArgs.Empty);
        } 
        else if (pennedSheepNumber == realTimeSheepNumber & !levelComplete) {
            EndLevelSuccess();
        }

    }

    private void EndLevelSuccess() {
        DisablePlayer();
        levelComplete = true;
        levelRemainingTime = levelTimer;

        playerScore = CalculatePlayerScore(pennedSheepNumber, initialSheepNumber, levelRemainingTime);
        playerBones = CalculatePlayerBones(playerScore);

        if (playerScore > highScore) {
            // Calculate how many new bones are added
            int newBones = playerBones - highestBones;

            // Add bones to player
            Player.Instance.GivePlayerBones(newBones);

            // Save high Score & Trophies
            SavePlayerScores();
        }

        OnLevelSucceeded?.Invoke(this, new OnLevelSucceededEventArgs {
            initialSheepNumber = initialSheepNumber,
            pennedSheepNumber = pennedSheepNumber,
            levelRemainingTime = levelRemainingTime,
            playerScore = playerScore,
            playerTrophies = playerBones
        });

    }

    private void SortSheepPennedByType(Sheep sheep) {
        if(sheep is WhiteSheep) {
            whitePennedSheepNumber++;
        }
        if(sheep is BlackSheep) {
            blackPennedSheepNumber++;
        }
    }

    private int CalculatePlayerBones(float playerScore) {
        if (playerScore >= platScoreTreshold) {
            playerBones = 4;
            platBone = true;
            goldBone = true;
            silverBone = true;
            bronzeBone = true;
        }
        if (playerScore >= goldScoreTreshold & playerScore < platScoreTreshold) {
            playerBones = 3;
            goldBone = true;
            silverBone = true;
            bronzeBone = true;
        }
        if (playerScore >= silverScoreTreshold & playerScore < goldScoreTreshold) {
            playerBones = 2;
            silverBone = true;
            bronzeBone = true;
        }
        if (playerScore >= bronzeScoreTreshold & playerScore < silverScoreTreshold) {
            playerBones = 1;
            bronzeBone = true;
        }
        if (playerScore < bronzeScoreTreshold) {
            playerBones = 0;
        }
        return playerBones;
    }

    private int CalculatePlayerScore(int pennedSheepNumber, int initialSheepNumber, float remainingTime) {
        int playerScore = (int)Mathf.Floor(remainingTime) * timeScore + 
            whiteSheepScore * pennedSheepNumber +
            blackSheepScore * pennedSheepNumber;
        return playerScore;
    }

    public int GetPennedSheepNumber() {
        return pennedSheepNumber;
    }

    public int GetInitialSheepNumber() {
        return initialSheepNumber;
    }

    public float GetLevelTimeLimit() {
        return levelTimeLimit;
    }

    public float GetLevelTimer() {
        return levelTimer;
    }

    private void LoadPlayerScores() {
        highScore = ES3.Load((levelSO.levelName + "_highScore"), defaultHighScore);
        highestBones = ES3.Load((levelSO.levelName + "_highestBones"), defaultHighestBones);
    }

    private void SavePlayerScores() {
        ES3.Save((levelSO.levelName + "_highScore"), playerScore);
        ES3.Save((levelSO.levelName + "_highestBones"), playerBones);
        ES3.Save((levelSO.levelName + "_completed"), true);
        ES3.Save((levelSO.levelName + "_pennedSheep"), pennedSheepNumber);
        ES3.Save((levelSO.levelName + "_bestTime"), levelRemainingTime);

        SavePlayerBones();
    }

    private void SavePlayerBones() {
        // Check if player had a bronze reward 
        if (!ES3.Load(levelSO.levelName + "_bronzeBone", false)) {
            // if not save new reward
            ES3.Save((levelSO.levelName + "_bronzeBone"), bronzeBone);
        }
        if (!ES3.Load(levelSO.levelName + "_silverBone", false)) {
            // if not save new reward
            ES3.Save((levelSO.levelName + "_silverBone"), silverBone);
        }
        if (!ES3.Load(levelSO.levelName + "_goldBone", false)) {
            // if not save new reward
            ES3.Save((levelSO.levelName + "_goldBone"), goldBone);
        }
        if (!ES3.Load(levelSO.levelName + "_platBone", false)) {
            // if not save new reward
            ES3.Save((levelSO.levelName + "_platBone"), platBone);
        }

    }

    private void DisablePlayer() {
        Player.Instance.gameObject.GetComponent<PlayerMovement>().SetCanMove(false);
        if (Player.Instance.gameObject.TryGetComponent<PlayerBark>(out PlayerBark playerBark)) {
            playerBark.enabled = false;
        }
        if (Player.Instance.gameObject.TryGetComponent<PlayerGrowl>(out PlayerGrowl playerGrowl)) {
            playerGrowl.enabled = false;
        }
        if (Player.Instance.gameObject.TryGetComponent<PlayerRun>(out PlayerRun playerRun)) {
            playerRun.enabled = false;
        }
    }
}
