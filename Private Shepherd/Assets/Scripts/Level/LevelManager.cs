using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private List<LevelCutScene> cutScenes = new List<LevelCutScene>();

    [SerializeField] private bool tutorial;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;

    private Sheep[] initialSheepsInLevel;
    [SerializeField] private LevelSO levelSO;

    private int platScoreTreshold;
    private int silverScoreTreshold;
    private int goldScoreTreshold;
    private int bronzeScoreTreshold;

    private int initialSheepNumber;
    private int realTimeSheepNumber;
    private int pennedSheepNumber = 0;

    private int initialWhiteSheepNumber;
    private int initialBlackSheepNumber;
    private int initialRedSheepNumber;
    private int initialGreenSheepNumber;
    private int initialBlueSheepNumber;
    private int initialGoldSheepNumber;
    private int initialGoatSheepNumber;

    private int whitePennedSheepNumber;
    private int goldPennedSheepNumber;
    private int blackPennedSheepNumber;
    private int bluePennedSheepNumber;
    private int greenPennedSheepNumber;
    private int redPennedSheepNumber;
    private int goatPennedSheepNumber;

    private float levelTimeLimit;
    private float levelTimer;
    private float levelRemainingTime;
    private int timeScore;

    private float maxSheepScore;
    private float maxTimeScore;
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
    private int goatSheepScore;

    [SerializeField] private SheepSO whiteSheepSO;
    [SerializeField] private SheepSO blackSheepSO;
    [SerializeField] private SheepSO goldSheepSO;
    [SerializeField] private SheepSO blueSheepSO;
    [SerializeField] private SheepSO redSheepSO;
    [SerializeField] private SheepSO greenSheepSO;
    [SerializeField] private SheepSO goatSheepSO;

    private bool levelComplete;
    private bool bronzeBone;
    private bool silverBone;
    private bool goldBone;
    private bool platBone;

    public event EventHandler<OnScoreUpdateEventArgs> OnScoreUpdate;
    public event EventHandler<OnLevelSucceededEventArgs> OnLevelSucceeded;
    public event EventHandler OnLevelFailed;
    public event EventHandler OnCutSceneEnter;
    public event EventHandler OnCutSceneExit;

    public class OnScoreUpdateEventArgs : EventArgs {
        public int realTimeSheepNumber;
        public int pennedSheepNumber;
    }

    public class OnLevelSucceededEventArgs : EventArgs {
        public int initialSheepNumber;
        public int initialWhiteSheepNumber;
        public int initialBlackSheepNumber;
        public int initialRedSheepNumber;
        public int initialGreenSheepNumber;
        public int initialBlueSheepNumber;
        public int initialGoldSheepNumber;
        public int initialGoatSheepNumber;

        public int pennedSheepNumber;
        public int pennedWhiteSheepNumber;
        public int pennedBlackSheepNumber;
        public int pennedRedSheepNumber;
        public int pennedGreenSheepNumber;
        public int pennedBlueSheepNumber;
        public int pennedGoldSheepNumber;
        public int pennedGoatSheepNumber;

        public float levelRemainingTime;
        public int playerScore;
        public int playerTrophies;
    }

    private void Awake() {
        Instance = this;

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;

        // Initialise sheeps
        InitializeSheep();

        // Initialise timers
        levelTimeLimit = levelSO.levelTimeLimit;
        levelTimer = levelTimeLimit;
        timeScore = levelSO.timeScore;


        // Initialise score thresholds
        InitializeScoreTresholds();

        // Initialize cutScenes
        LevelCutScene[] cutScenesArray = GetComponents<LevelCutScene>();
        foreach (LevelCutScene scene in cutScenesArray) {
            cutScenes.Add(scene);
            scene.OnCutSceneEnter += Scene_OnCutSceneEnter;
            scene.OnCutSceneExit += Scene_OnCutSceneExit;
        }
    }

    private void Start() {
        LoadPlayerScores();
    }

    private void Update() {

        if(Input.GetKeyDown(KeyCode.T)) {
            EndLevelSuccess();
        }

        if(LevelIntroCutScene.Instance.GetCutSceneInProgress()) {
            return;
        }
        levelTimer -= Time.deltaTime;

        if (levelTimer < 0 & !levelComplete) {
            DisablePlayer();
            levelComplete = true;
            OnLevelFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void InitializeSheep() {
        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();
        initialSheepNumber = initialSheepsInLevel.Length;
        realTimeSheepNumber = initialSheepNumber;

        initialWhiteSheepNumber = 0;
        initialBlackSheepNumber = 0;
        initialBlueSheepNumber = 0;
        initialGreenSheepNumber = 0;
        initialRedSheepNumber = 0;
        initialGoldSheepNumber = 0;
        initialGoatSheepNumber = 0;

        whiteSheepScore = whiteSheepSO.sheepScore;
        blackSheepScore = blackSheepSO.sheepScore;
        blueSheepScore = blueSheepSO.sheepScore;
        redSheepScore = redSheepSO.sheepScore;
        goldSheepScore = goldSheepSO.sheepScore;
        greenSheepScore = greenSheepSO.sheepScore;
        goatSheepScore = goatSheepSO.sheepScore;

        foreach (Sheep sheep in initialSheepsInLevel) {
            sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;

            if (sheep.GetSheepType() == SheepType.whiteSheep) {
                initialWhiteSheepNumber++;
            }
            if (sheep.GetSheepType() == SheepType.blackSheep) {
                initialBlackSheepNumber++;
            }
            if (sheep.GetSheepType() == SheepType.redSheep) {
                initialRedSheepNumber++;
            }
            if (sheep.GetSheepType() == SheepType.blueSheep) {
                initialBlueSheepNumber++;
            }
            if (sheep.GetSheepType() == SheepType.greenSheep) {
                initialGreenSheepNumber++;
            }
            if (sheep.GetSheepType() == SheepType.goldSheep) {
                initialGoldSheepNumber++;
            }

            if (sheep.GetSheepType() == SheepType.goat) {
                initialGoatSheepNumber++;
            }
        }

    }

    private void InitializeScoreTresholds() {

        if (timeScore < 5) {
            // Sheep based level
            maxSheepScore = initialWhiteSheepNumber * whiteSheepScore +
                initialBlackSheepNumber * blackSheepScore +
                initialRedSheepNumber * redSheepScore +
                initialGreenSheepNumber * greenSheepScore +
                initialBlueSheepNumber * blueSheepScore +
                initialGoldSheepNumber * goldSheepScore +
                initialGoatSheepNumber * goatSheepScore;

            platScoreTreshold = ((int)((2 * maxSheepScore / 3 + levelTimeLimit) / 10)) * 10;
            goldScoreTreshold = ((int)((9 * platScoreTreshold / 10) / 10)) * 10;
            silverScoreTreshold = ((int)((4 * platScoreTreshold / 5) / 10)) * 10;
            bronzeScoreTreshold = ((int)((7 * platScoreTreshold / 10) / 10)) * 10;

        }
        else if(timeScore> 5) {
            // Time based level

            maxSheepScore = initialWhiteSheepNumber * whiteSheepScore +
                initialBlackSheepNumber * blackSheepScore +
                initialRedSheepNumber * redSheepScore +
                initialGreenSheepNumber * greenSheepScore +
                initialBlueSheepNumber * blueSheepScore +
                initialGoldSheepNumber * goldSheepScore +
                initialGoatSheepNumber * goatSheepScore;

            maxTimeScore = levelTimeLimit * timeScore;

            platScoreTreshold = ((int)((maxSheepScore + 2 * maxTimeScore / 3) / 10)) * 10;
            goldScoreTreshold = ((int)((9 * platScoreTreshold / 10) / 10)) * 10;
            silverScoreTreshold = ((int)((4 * platScoreTreshold / 5) / 10)) * 10;
            bronzeScoreTreshold = ((int)((7 * platScoreTreshold / 10) / 10)) * 10;

        }
        else if (timeScore == 5) {
            // Time based level

            maxSheepScore = initialWhiteSheepNumber * whiteSheepScore +
                initialBlackSheepNumber * blackSheepScore +
                initialRedSheepNumber * redSheepScore +
                initialGreenSheepNumber * greenSheepScore +
                initialBlueSheepNumber * blueSheepScore +
                initialGoldSheepNumber * goldSheepScore +
                initialGoatSheepNumber * goatSheepScore;

            maxTimeScore = levelTimeLimit * timeScore;

            platScoreTreshold = ((int)((maxSheepScore / 2 + maxTimeScore / 2) / 10)) * 10;
            goldScoreTreshold = ((int)((9 * platScoreTreshold / 10) / 10)) * 10;
            silverScoreTreshold = ((int)((4 * platScoreTreshold / 5) / 10)) * 10;
            bronzeScoreTreshold = ((int)((7 * platScoreTreshold / 10) / 10)) * 10;

        }


        playerBones = 0;
        playerScore = 0;

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

        if(!tutorial) {
            playerScore = CalculatePlayerScore(levelRemainingTime);
            playerBones = CalculatePlayerBones(playerScore);

            if (playerScore > highScore) {
                // Calculate how many new bones are added
                int newBones = playerBones - highestBones;

                // Add bones to player
                Player.Instance.GivePlayerBones(newBones);

                // Save high Score & Trophies
                SavePlayerScores();
            }
        }

        OnLevelSucceeded?.Invoke(this, new OnLevelSucceededEventArgs {
            initialSheepNumber = initialSheepNumber,
            initialBlackSheepNumber = initialBlackSheepNumber,
            initialBlueSheepNumber = initialBlueSheepNumber,
            initialGoldSheepNumber = initialGoldSheepNumber,
            initialGreenSheepNumber = initialGreenSheepNumber,
            initialRedSheepNumber = initialRedSheepNumber,
            initialWhiteSheepNumber = initialWhiteSheepNumber,
            initialGoatSheepNumber = initialGoatSheepNumber,

            pennedSheepNumber = pennedSheepNumber,
            pennedBlackSheepNumber = blackPennedSheepNumber,
            pennedBlueSheepNumber = bluePennedSheepNumber,
            pennedGoldSheepNumber = goldPennedSheepNumber,
            pennedGreenSheepNumber = greenPennedSheepNumber,
            pennedRedSheepNumber = redPennedSheepNumber,
            pennedWhiteSheepNumber = whitePennedSheepNumber,
            pennedGoatSheepNumber = goatPennedSheepNumber,

            levelRemainingTime = levelRemainingTime,
            playerScore = playerScore,
            playerTrophies = playerBones

        });

    }

    private void SortSheepPennedByType(Sheep sheep) {
        if(sheep.GetSheepType() == SheepType.whiteSheep) {
            whitePennedSheepNumber++;
        }
        if(sheep.GetSheepType() == SheepType.blackSheep) {
            blackPennedSheepNumber++;
        }
        if (sheep.GetSheepType() == SheepType.redSheep) {
            redPennedSheepNumber++;
        }
        if (sheep.GetSheepType() == SheepType.blueSheep) {
            bluePennedSheepNumber++;
        }
        if (sheep.GetSheepType() == SheepType.greenSheep) {
            greenPennedSheepNumber++;
        }
        if (sheep.GetSheepType() == SheepType.goldSheep) {
            goldPennedSheepNumber++;
        }
        if (sheep.GetSheepType() == SheepType.goat) {
            goatPennedSheepNumber++;
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

    private int CalculatePlayerScore(float remainingTime) {
        int playerScore = (int)Mathf.Floor(remainingTime) * timeScore +
            whiteSheepScore * whitePennedSheepNumber +
            blackSheepScore * blackPennedSheepNumber +
            redSheepScore * redPennedSheepNumber +
            blueSheepScore * bluePennedSheepNumber +
            greenSheepScore * greenPennedSheepNumber+
            goatSheepScore * goatPennedSheepNumber +
            goldSheepScore * goldPennedSheepNumber;
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

    private void Scene_OnCutSceneExit(object sender, EventArgs e) {
        OnCutSceneExit?.Invoke(this, EventArgs.Empty);
    }

    private void Scene_OnCutSceneEnter(object sender, EventArgs e) {
        OnCutSceneEnter?.Invoke(this, EventArgs.Empty);
    }

    public LevelSO GetLevelSO() {
        return levelSO;
    }

    public int GetBronzeScoreTreshold() {
        return bronzeScoreTreshold;
    }

    public int GetSilverScoreTreshold() {
        return silverScoreTreshold;
    }

    public int GetGoldScoreTreshold() {
        return goldScoreTreshold;
    }

    public int GetPlatScoreTreshold() {
        return platScoreTreshold;
    }
}
