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
    private int playerTrophies;

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
            playerTrophies = 5;
        }
        if (playerScore >= goldScoreTreshold & playerScore < platScoreTreshold) {
            playerTrophies = 4;
        }
        if (playerScore >= silverScoreTreshold & playerScore < goldScoreTreshold) {
            playerTrophies = 3;
        }
        if (playerScore >= bronzeScoreTreshold & playerScore < silverScoreTreshold) {
            playerTrophies = 2;
        }
        if (playerScore < silverScoreTreshold) {
            playerTrophies = 1;
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

    public int GetTwoStarScore() {
        return silverScoreTreshold;
    }

    public int GetThreeStarScore() {
        return goldScoreTreshold;
    }
}
