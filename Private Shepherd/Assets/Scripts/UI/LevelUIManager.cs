using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    [SerializeField] private LevelSO levelSO;

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

    [SerializeField] GameObject whiteSheepIcon;
    [SerializeField] GameObject redSheepIcon;
    [SerializeField] GameObject greenSheepIcon;
    [SerializeField] GameObject blueSheepIcon;
    [SerializeField] GameObject blackSheepIcon;
    [SerializeField] GameObject goldSheepIcon;

    [SerializeField] TextMeshProUGUI whiteInitialSheepNumberText;
    [SerializeField] TextMeshProUGUI blackInitialSheepNumberText;
    [SerializeField] TextMeshProUGUI blueInitialSheepNumberText;
    [SerializeField] TextMeshProUGUI redInitialSheepNumberText;
    [SerializeField] TextMeshProUGUI greenInitialSheepNumberText;
    [SerializeField] TextMeshProUGUI goldInitialSheepNumberText;

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

    #region SHEEP TYPES
    private int whiteSheepScore;
    private int blackSheepScore;
    private int goldSheepScore;
    private int blueSheepScore;
    private int redSheepScore;
    private int greenSheepScore;

    private int whitePennedSheepNumber;
    private int blackPennedSheepNumber;
    private int goldPennedSheepNumber;
    private int bluePennedSheepNumber;
    private int redPennedSheepNumber;
    private int greenPennedSheepNumber;

    private int whiteInitialSheepNumber;
    private int blackInitialSheepNumber;
    private int goldInitialSheepNumber;
    private int blueInitialSheepNumber;
    private int redInitialSheepNumber;
    private int greenInitialSheepNumber;

    #endregion

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
        if(blackSheepScoreText != null) {
            blackSheepScoreText.text = "X" + blackSheepScore.ToString();
        }

        redSheepScore = levelSO.redSheepScore;
        if (redSheepScoreText != null) {
            redSheepScoreText.text = "X" + redSheepScore.ToString();
        }

        blueSheepScore = levelSO.blueSheepScore;
        if (blueSheepScoreText != null) {
            blueSheepScoreText.text = "X" + blueSheepScore.ToString();
        }

        greenSheepScore = levelSO.greenSheepScore;
        if (greenSheepScoreText != null) {
            greenSheepScoreText.text = "X" + greenSheepScore.ToString();
        }

        goldSheepScore = levelSO.goldSheepScore;
        if (goldSheepScoreText != null) {
            goldSheepScoreText.text = "X" + goldSheepScore.ToString();
        }
    }

    private void DisableLevelSuccessGO() {
        mainMenuButton.SetActive(false);
        tryAgainButton.SetActive(false);
        continueButton.SetActive(false);
        clockIcon.SetActive(false);
        bronzeTrophy.SetActive(false);
        silverTrophy.SetActive(false);
        goldTrophy.SetActive(false);
        platTrophy.SetActive(false);
        foreach (GameObject t in trophyContours) {
            t.SetActive(false);
        }

        whiteSheepIcon.SetActive(false);
        blackSheepIcon.SetActive(false);
        greenSheepIcon.SetActive(false);
        blueSheepIcon.SetActive(false);
        redSheepIcon.SetActive(false);
        goldSheepIcon.SetActive(false);

        whiteInitialSheepNumberText.gameObject.SetActive(false);
        whitePennedSheepNumberText.gameObject.SetActive(false);
        blueInitialSheepNumberText.gameObject.SetActive(false);
        bluePennedSheepNumberText.gameObject.SetActive(false);
        redInitialSheepNumberText.gameObject.SetActive(false);
        redPennedSheepNumberText.gameObject.SetActive(false);
        greenInitialSheepNumberText.gameObject.SetActive(false);
        greenPennedSheepNumberText.gameObject.SetActive(false);
        blackInitialSheepNumberText.gameObject.SetActive(false);
        blackPennedSheepNumberText.gameObject.SetActive(false);
        goldInitialSheepNumberText.gameObject.SetActive(false);
        goldPennedSheepNumberText.gameObject.SetActive(false);

        playerDynamicScoreText.gameObject.SetActive(false);
        levelRemainingTimeText.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        timeScoreText.gameObject.SetActive(false);

        whiteSheepScoreText.gameObject.SetActive(false);
        blueSheepScoreText.gameObject.SetActive(false);
        redSheepScoreText.gameObject.SetActive(false);
        greenSheepScoreText.gameObject.SetActive(false);
        blackSheepScoreText.gameObject.SetActive(false);
        goldSheepScoreText.gameObject.SetActive(false);

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

        whitePennedSheepNumber = e.pennedWhiteSheepNumber;
        redPennedSheepNumber = e.pennedRedSheepNumber;
        bluePennedSheepNumber = e.pennedBlueSheepNumber;
        goldPennedSheepNumber = e.pennedGoldSheepNumber;
        blackPennedSheepNumber = e.pennedBlackSheepNumber;
        greenPennedSheepNumber = e.pennedGreenSheepNumber;

        whiteInitialSheepNumber = e.initialWhiteSheepNumber;
        redInitialSheepNumber = e.initialRedSheepNumber;
        blueInitialSheepNumber = e.initialBlueSheepNumber;
        greenInitialSheepNumber = e.initialGreenSheepNumber;
        blackInitialSheepNumber = e.initialBlackSheepNumber;
        goldInitialSheepNumber = e.initialGoldSheepNumber;

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

        float pennedSheepTextAnimationTime = .5f;
        if (whiteInitialSheepNumber != 0) {
            whiteSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, whiteInitialSheepNumber, whitePennedSheepNumber, whiteInitialSheepNumberText, whitePennedSheepNumberText, whiteSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        if (blackInitialSheepNumber != 0) {
            blackSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, blackInitialSheepNumber, blackPennedSheepNumber, blackInitialSheepNumberText, blackPennedSheepNumberText, blackSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        if (blueInitialSheepNumber != 0) {
            blueSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, blueInitialSheepNumber, bluePennedSheepNumber, blueInitialSheepNumberText, bluePennedSheepNumberText, blueSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        if (greenInitialSheepNumber != 0) {
            greenSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, greenInitialSheepNumber, greenPennedSheepNumber, greenInitialSheepNumberText, greenPennedSheepNumberText, greenSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        if (redInitialSheepNumber != 0) {
            redSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, redInitialSheepNumber, redPennedSheepNumber, redInitialSheepNumberText, redPennedSheepNumberText, redSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        if (goldInitialSheepNumber != 0) {
            goldSheepIcon.SetActive(true);
            DisplaySheepLeft(pennedSheepTextAnimationTime, goldInitialSheepNumber, goldPennedSheepNumber, goldInitialSheepNumberText, goldPennedSheepNumberText, goldSheepScoreText);
            yield return new WaitForSeconds(pennedSheepTextAnimationTime);
        }

        float timeToDisplayFinalScore = .5f;
        yield return new WaitForSeconds(timeToDisplayFinalScore);
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
        float timeBetweenSheepTypes = .3f;

        if (whiteInitialSheepNumber != 0) {
            while (whitePennedSheepNumber > 0) {
                whitePennedSheepNumber--;
                whitePennedSheepNumberText.text = whitePennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + whiteSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (blackInitialSheepNumber != 0) {
            while (blackPennedSheepNumber > 0) {
                blackPennedSheepNumber--;
                blackPennedSheepNumberText.text = blackPennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + blackSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (blueInitialSheepNumber != 0) {
            while (bluePennedSheepNumber > 0) {
                bluePennedSheepNumber--;
                bluePennedSheepNumberText.text = bluePennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + blueSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (greenInitialSheepNumber != 0) {
            while (greenPennedSheepNumber > 0) {
                greenPennedSheepNumber--;
                greenPennedSheepNumberText.text = greenPennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + greenSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (redInitialSheepNumber != 0) {
            while (redPennedSheepNumber > 0) {
                redPennedSheepNumber--;
                redPennedSheepNumberText.text = redPennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + redSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (goldInitialSheepNumber != 0) {
            while (goldPennedSheepNumber > 0) {
                goldPennedSheepNumber--;
                goldPennedSheepNumberText.text = goldPennedSheepNumber.ToString();
                playerDynamicScoreText.text = (playerDynamicScore + goldSheepScore).ToString();
                yield return new WaitForSeconds(refreshRate);
            }
        }

        scoreTransferred = true;
    }

    private void DisplaySheepLeft(float animationTime, int initialSheepTypeNumber, int pennedSheepTypeNumber, TextMeshProUGUI initialSheepNumberText, TextMeshProUGUI pennedSheepNumberText, TextMeshProUGUI sheepScoreText) {
        initialSheepNumberText.gameObject.SetActive(true);
        pennedSheepNumberText.gameObject.SetActive(true);
        sheepScoreText.gameObject.SetActive(true);
        initialSheepNumberText.text = "/" + initialSheepTypeNumber.ToString();

        float refreshRate = animationTime / pennedSheepTypeNumber;
        int displayIncrement = 1;

        StartCoroutine(DisplayIntTextOverTime(pennedSheepNumberText, 0, initialSheepTypeNumber, displayIncrement, refreshRate, true, ""));
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
