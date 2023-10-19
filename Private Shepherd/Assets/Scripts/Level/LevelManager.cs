using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float levelTimeLimit;
    private float levelTimer;
    private float levelRemainingTime;

    private int playerScore;
    private int defaultHighScore = 0;
    private int highScore;
    private int playerTrophies;
    private int highTrophies;
    private int defaultHighTrophies;

    private int whiteSheepScore;

    private bool levelComplete;
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
        whiteSheepScore = whiteSheepSO.sheepScore;

        foreach (Sheep sheep in initialSheepsInLevel) {
            sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;
        }

        // Initialise score thresholds
        bronzeScoreTreshold = levelSO.bronzeScoreTreshold;   
        silverScoreTreshold = levelSO.silverScoreTreshold;
        goldScoreTreshold = levelSO.goldScoreTreshold;
        platScoreTreshold = levelSO.platScoreTreshold;

        // Initialise timers
        levelTimeLimit = levelSO.levelTimeLimit;
        levelTimer = levelTimeLimit;

        playerTrophies = 0;
        playerScore = 0;
    }

    private void Start() {
        LoadPlayerScores();
    }

    private void Update() {
        levelTimer -= Time.deltaTime;

        if (levelTimer < 0 & !levelComplete) {
            levelComplete = true;
            OnLevelFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, Sheep.OnSheepEnterScoreZoneEventArgs e) {
        pennedSheepNumber++;

        OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventArgs {
            realTimeSheepNumber = realTimeSheepNumber,
            pennedSheepNumber = pennedSheepNumber
        });

        if (pennedSheepNumber == realTimeSheepNumber & !levelComplete) {
            levelComplete = true;
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
        levelComplete = true;
        levelRemainingTime = levelTimer;

        playerScore = CalculatePlayerScore(pennedSheepNumber, initialSheepNumber, levelRemainingTime);

        if (playerScore >= platScoreTreshold) {
            playerTrophies = 4;
        }
        if (playerScore >= goldScoreTreshold & playerScore < platScoreTreshold) {
            playerTrophies = 3;
        }
        if (playerScore >= silverScoreTreshold & playerScore < goldScoreTreshold) {
            playerTrophies = 2;
        }
        if (playerScore >= bronzeScoreTreshold & playerScore < silverScoreTreshold) {
            playerTrophies = 1;
        }
        if (playerScore < bronzeScoreTreshold) {
            playerTrophies = 0;
        }

        if (playerScore > highScore) {
            // Calculate how many new bones are added
            int newTrophies = playerTrophies - highTrophies;

            // Add bones to player
            Player.Instance.GivePlayerBones(newTrophies);

            // Save high Score & Trophies
            SavePlayerScores();
        }

        OnLevelSucceeded?.Invoke(this, new OnLevelSucceededEventArgs {
            initialSheepNumber = initialSheepNumber,
            pennedSheepNumber = pennedSheepNumber,
            levelRemainingTime = levelRemainingTime,
            playerScore = playerScore,
            playerTrophies = playerTrophies
        });

    }

    private int CalculatePlayerScore(int pennedSheepNumber, int initialSheepNumber, float remainingTime) {
        int playerScore = (int)Mathf.Floor(remainingTime) + whiteSheepScore * pennedSheepNumber;
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
        highTrophies = ES3.Load((levelSO.levelName + "_highTrophies"), defaultHighTrophies);
    }

    private void SavePlayerScores() {
        ES3.Save((levelSO.levelName + "_highScore"), playerScore);
        ES3.Save((levelSO.levelName + "_highTrophies"), playerTrophies);
        ES3.Save((levelSO.levelName + "_completed"), true);
        ES3.Save((levelSO.levelName + "_pennedSheep"), pennedSheepNumber);
        ES3.Save((levelSO.levelName + "_bestTime"), levelRemainingTime);
    }
}
