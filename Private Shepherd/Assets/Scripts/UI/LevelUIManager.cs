using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private SheepSO whiteSheepSO;

    [SerializeField] GameObject levelFailedUI;
    [SerializeField] GameObject levelPlayUI;
    [SerializeField] GameObject levelSuccededUI;

    [SerializeField] LevelManager levelManager;

    [SerializeField] GameObject mainMenuButton;
    [SerializeField] GameObject tryAgainButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject clockIcon;
    [SerializeField] TextMeshProUGUI levelRemainingTimeText;
    [SerializeField] TextMeshProUGUI timeScoreText;
    [SerializeField] TextMeshProUGUI whiteSheepScoreText;
    [SerializeField] TextMeshProUGUI blackSheepScoreText;
    [SerializeField] TextMeshProUGUI blueSheepScoreText;
    [SerializeField] TextMeshProUGUI redSheepScoreText;
    [SerializeField] TextMeshProUGUI greenSheepScoreText;
    [SerializeField] TextMeshProUGUI goldSheepScoreText;
    [SerializeField] GameObject sheepIcon;
    [SerializeField] TextMeshProUGUI initialSheepNumberText;
    [SerializeField] TextMeshProUGUI whitePennedSheepNumberText;
    [SerializeField] TextMeshProUGUI blackPennedSheepNumberText;
    [SerializeField] TextMeshProUGUI bluePennedSheepNumberText;
    [SerializeField] TextMeshProUGUI redPennedSheepNumberText;
    [SerializeField] TextMeshProUGUI greenPennedSheepNumberText;
    [SerializeField] TextMeshProUGUI goldPennedSheepNumberText;
    [SerializeField] TextMeshProUGUI playerDynamicScoreText;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject bronzeTrophy;
    [SerializeField] GameObject silverTrophy;
    [SerializeField] GameObject goldTrophy;
    [SerializeField] GameObject platTrophy;
    [SerializeField] GameObject[] trophyContours;

    [SerializeField] GameObject ProgressionBarGameObject;
    [SerializeField] Image bronzeProgressionBar;
    [SerializeField] Image silverProgressionBar;
    [SerializeField] Image goldProgressionBar;
    [SerializeField] Image platProgressionBar;

    private int initialSheepNumber;
    private int pennedSheepNumber;
    private int finalPlayerScore;
    private int playerTrophies;
    private float levelRemainingTime;

    private int playerDynamicScore;
    private float levelRemainingDynamicTime;
    private float timeScore;

    private int whiteSheepScore;
    private int blackSheepScore;
    private int goldSheepScore;
    private int blueSheepScore;
    private int redSheepScore;
    private int greenSheepScore;

    private int platScoreTreshold;
    private int silverScoreTreshold;
    private int goldScoreTreshold;
    private int bronzeScoreTreshold;

    private bool fillingProgressionBars;
    private bool scoreTransferred;
    private bool sheepDisplayed;

    private void Awake() {
        // Initialise score thresholds & sheep scores
        InitialiseScoreTresholds();
        InitialiseSheepScores();

        // Disable all level success gameObjects
        DisableLevelSuccessGO();

    }

    private void Start() {
        levelManager.OnLevelSucceeded += LevelManager_OnLevelSucceeded;
        levelManager.OnLevelFailed += LevelManager_OnLevelFailed;
    }

    private void Update() {
       // levelRemainingTimeText.text = levelRemainingDynamicTime.ToString();

        if (fillingProgressionBars) {
            // playerDynamicScoreText.text = playerDynamicScore.ToString();
            playerDynamicScore = (int)Mathf.Ceil(float.Parse(playerDynamicScoreText.text));
            FillProgressionBars();
        }
    }

    private void InitialiseScoreTresholds() {
        bronzeScoreTreshold = levelSO.bronzeScoreTreshold;
        silverScoreTreshold = levelSO.silverScoreTreshold;
        goldScoreTreshold = levelSO.goldScoreTreshold;
        platScoreTreshold = levelSO.platScoreTreshold;
    }

    private void InitialiseSheepScores() {
        timeScore = levelSO.timeScore;
        timeScoreText.text = "X" + levelSO.timeScore.ToString();

        whiteSheepScore = levelSO.whiteSheepScore;
        whiteSheepScoreText.text = "X" + whiteSheepScore.ToString();

        blackSheepScore = levelSO.blackSheepScore;
        blackSheepScoreText.text = "X" + blackSheepScore.ToString();

        redSheepScore = levelSO.redSheepScore;
        redSheepScoreText.text = "X" + redSheepScore.ToString();

        blueSheepScore = levelSO.blueSheepScore;
        blueSheepScoreText.text = "X" + blueSheepScore.ToString();

        greenSheepScore = levelSO.greenSheepScore;
        greenSheepScoreText.text = "X" + greenSheepScore.ToString();

        goldSheepScore = levelSO.goldSheepScore;
        goldSheepScoreText.text = "X" + goldSheepScore.ToString();
    }

    private void DisableLevelSuccessGO() {
        mainMenuButton.SetActive(false);
        tryAgainButton.SetActive(false);
        continueButton.SetActive(false);
        clockIcon.SetActive(false);
        sheepIcon.SetActive(false);
        bronzeTrophy.SetActive(false);
        silverTrophy.SetActive(false);
        goldTrophy.SetActive(false);
        platTrophy.SetActive(false);
        foreach (GameObject t in trophyContours) {
            t.SetActive(false);
        }
        initialSheepNumberText.gameObject.SetActive(false);
        whitePennedSheepNumberText.gameObject.SetActive(false);
        playerDynamicScoreText.gameObject.SetActive(false);
        levelRemainingTimeText.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        timeScoreText.gameObject.SetActive(false);
        whiteSheepScoreText.gameObject.SetActive(false);

        levelFailedUI.SetActive(false);
        levelPlayUI.SetActive(true);
        levelSuccededUI.SetActive(false);

        bronzeProgressionBar.fillAmount = 0;
        silverProgressionBar.fillAmount = 0;
        goldProgressionBar.fillAmount = 0;
        platProgressionBar.fillAmount = 0;
        ProgressionBarGameObject.SetActive(false);
    }

    private void FillProgressionBars() {
        bronzeProgressionBar.fillAmount = ((float)playerDynamicScore / (float)bronzeScoreTreshold);
        silverProgressionBar.fillAmount = (((float)playerDynamicScore - (float)bronzeScoreTreshold) / ((float)silverScoreTreshold - (float)bronzeScoreTreshold));
        goldProgressionBar.fillAmount = (((float)playerDynamicScore - (float)silverScoreTreshold) / ((float)goldScoreTreshold - (float)silverScoreTreshold));
        platProgressionBar.fillAmount = (((float)playerDynamicScore - (float)goldScoreTreshold) / ((float)platScoreTreshold - (float)goldScoreTreshold));

        if (bronzeProgressionBar.fillAmount == 1) {
            bronzeTrophy.SetActive(true);
        }
        if (silverProgressionBar.fillAmount == 1) {
            silverTrophy.SetActive(true);
        }
        if (goldProgressionBar.fillAmount == 1) {
            goldTrophy.SetActive(true);
        }
        if (platProgressionBar.fillAmount == 1) {
            platTrophy.SetActive(true);
        }
    }

    private void LevelManager_OnLevelFailed(object sender, System.EventArgs e) {
        levelPlayUI.SetActive(false);
        levelFailedUI.SetActive(true);
    }

    private void LevelManager_OnLevelSucceeded(object sender, LevelManager.OnLevelSucceededEventArgs e) {
        levelPlayUI.SetActive(false);
        levelSuccededUI.SetActive(true);

        levelRemainingTime = e.levelRemainingTime;
        finalPlayerScore = e.playerScore;
        playerTrophies = e.playerTrophies;
        initialSheepNumber = e.initialSheepNumber;
        pennedSheepNumber= e.pennedSheepNumber;

        StartCoroutine(DisplayPlayerScore());
    }

    private IEnumerator DisplayPlayerScore() {

        float timeToDisplayClock = 1f;
        yield return new WaitForSeconds(timeToDisplayClock);
        
        timeScoreText.gameObject.SetActive(true);
        clockIcon.SetActive(true);
        DisplayTimeLeft();

        float timeToDisplaySheep = 1f;
        yield return new WaitForSeconds(timeToDisplaySheep);

        sheepIcon.SetActive(true);
        float pennedSheepTextAnimationTime = 1f;
        DisplaySheepLeft(pennedSheepTextAnimationTime);

        float timeToDisplayFinalScore = .5f;
        yield return new WaitForSeconds(pennedSheepTextAnimationTime + timeToDisplayFinalScore);
        StartCoroutine(DisplayFinalScore());

        while (!scoreTransferred) {
            yield return null;
        }

        float timeToDisplayButtons = .5f;
        yield return new WaitForSeconds(timeToDisplayButtons);
        DisplayButtons();
    }

    private void DisplayButtons() {
        mainMenuButton.SetActive(true);
        tryAgainButton.SetActive(true);
        continueButton.SetActive(true);
    }

    private IEnumerator DisplayFinalScore() {
        finalScoreText.gameObject.SetActive(true);

        playerDynamicScoreText.gameObject.SetActive(true);
        foreach (GameObject t in trophyContours) {
            t.SetActive(true);
        }
        ProgressionBarGameObject.SetActive(true);

        float timeToStartTransferringScore = 1f;
        yield return new WaitForSeconds(timeToStartTransferringScore);

        // Transfer time score to final score
        playerDynamicScoreText.text = "0";
        int levelRemainingTimeInt = Mathf.FloorToInt(levelRemainingTime);
        float finalScoreTextAnimationTime = 2f;
        float refreshRate = .05f;
        float finalPlayerScoreWithTime = levelRemainingTime * timeScore;
        float scoreDisplayIncrement = calculateIncrementForTextAnimation(finalScoreTextAnimationTime, finalPlayerScoreWithTime, refreshRate);
        float timeDisplayIncrement = calculateIncrementForTextAnimation(finalScoreTextAnimationTime, levelRemainingTime, refreshRate);

        fillingProgressionBars = true;
        StartCoroutine(DisplayFloatTextOverTime(levelRemainingTimeText, levelRemainingTime, 0f, timeDisplayIncrement, refreshRate, false, " s"));
        StartCoroutine(DisplayFloatTextOverTime(playerDynamicScoreText, 0, finalPlayerScoreWithTime, scoreDisplayIncrement, refreshRate, true, ""));

        while (float.Parse(playerDynamicScoreText.text) < Mathf.Floor(finalPlayerScoreWithTime)) {
            yield return null;
        }
        playerDynamicScoreText.text = Mathf.Ceil(finalPlayerScoreWithTime).ToString();
        float timeToDisplaySheepScore = 1f;
        yield return new WaitForSeconds(timeToDisplaySheepScore);

        // Transfer sheep score to final score
        float transferSheepToScoreAnimTime = 2f;

        refreshRate = transferSheepToScoreAnimTime/pennedSheepNumber;
        StartCoroutine(TransferSheepToScore(refreshRate));
    }

    private IEnumerator TransferSheepToScore(float refreshRate) {
        int sheepAdded = 0;
        while (pennedSheepNumber > 0) {
            pennedSheepNumber--;
            sheepAdded++;
            whitePennedSheepNumberText.text = pennedSheepNumber.ToString();
            playerDynamicScoreText.text = (playerDynamicScore + whiteSheepScore).ToString();
            yield return new WaitForSeconds(refreshRate);
        }
        scoreTransferred = true;
    }

    private void DisplaySheepLeft(float animationTime) {
        initialSheepNumberText.gameObject.SetActive(true);
        whitePennedSheepNumberText.gameObject.SetActive(true);
        whiteSheepScoreText.gameObject.SetActive(true);
        initialSheepNumberText.text = "/" + initialSheepNumber.ToString();

        float refreshRate = animationTime / pennedSheepNumber;
        int displayIncrement = 1;

        StartCoroutine(DisplayIntTextOverTime(whitePennedSheepNumberText, 0, initialSheepNumber, displayIncrement, refreshRate, true, ""));
    }

    private void DisplayTimeLeft() {
        levelRemainingTimeText.gameObject.SetActive(true);

        float remainingTimeTextAnimationTime = 1f;
        float refreshRate = .03f;
        float displayIncrement = calculateIncrementForTextAnimation(remainingTimeTextAnimationTime, levelRemainingTime, refreshRate);

        StartCoroutine(DisplayFloatTextOverTime(levelRemainingTimeText, 0f, levelRemainingTime, displayIncrement, refreshRate, true, " s"));
    }

    private IEnumerator DisplayFloatTextOverTime(TextMeshProUGUI text, float initialFloat, float targetFloat, float displayIncrement, float refreshRate, bool addition, string endString) {
        
        float dynamicFloat = initialFloat;
        float zeroPauseTime = 0.5f;
        yield return new WaitForSeconds(zeroPauseTime);

        if (addition) {
            while ((dynamicFloat + displayIncrement) <= targetFloat) {
                dynamicFloat += displayIncrement;
                text.text = dynamicFloat.ToString("F2") + endString;
                yield return new WaitForSeconds(refreshRate);
            }
            text.text = targetFloat.ToString("F2") + endString;
        } else {
            while ((dynamicFloat - displayIncrement) >= 0) {
                dynamicFloat -= displayIncrement;
                text.text = dynamicFloat.ToString("F2") + endString;
                yield return new WaitForSeconds(refreshRate);
            }
            text.text = "0" + endString;
        }
    }

    private IEnumerator DisplayIntTextOverTime(TextMeshProUGUI text, int initialInt, int targetInt, int displayIncrement, float refreshRate, bool addition, string endString) {
        int dynamicInt = initialInt;
        float zeroPauseTime = 0.5f;
        yield return new WaitForSeconds(zeroPauseTime);

        if (addition) {
            while (dynamicInt < targetInt) {
                dynamicInt += displayIncrement;
                text.text = dynamicInt.ToString() + endString;
                yield return new WaitForSeconds(refreshRate);
            }
            text.text = targetInt.ToString() + endString;
        } else {
            while (dynamicInt > 0) {
                dynamicInt -= displayIncrement;
                text.text = dynamicInt.ToString() + endString;
                yield return new WaitForSeconds(refreshRate);
            }
            text.text = "0" + endString;
        }
    }

    private float calculateIncrementForTextAnimation(float animationTime, float targetFloat, float refreshRate) {
        float displayIncrement = targetFloat * refreshRate / animationTime;
        return displayIncrement;
    }


}
