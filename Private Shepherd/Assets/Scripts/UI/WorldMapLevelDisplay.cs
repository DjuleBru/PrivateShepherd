using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapLevelDisplay : MonoBehaviour
{
    [SerializeField] private Collider2D collider;
    [SerializeField] private GameObject levelDisplayUI;
    [SerializeField] private GameObject levelUnlockUI;
    [SerializeField] private QuestGiver questGiver;

    [SerializeField] private Button playButton;

    private bool playerIsInTriggerZone;

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

    #region LEVEL PARAMETERS

    private int levelSOCount = 0;

    private LevelSO levelSO;
    [SerializeField] private TextMeshProUGUI levelNameLocked;
    [SerializeField] private TextMeshProUGUI levelNameUnlocked;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI bonePriceText;
    [SerializeField] private TextMeshProUGUI initialTime;

    [SerializeField] private TextMeshProUGUI bronzeScoreTresholdText;
    [SerializeField] private TextMeshProUGUI silverScoreTresholdText;
    [SerializeField] private TextMeshProUGUI goldScoreTresholdText;
    [SerializeField] private TextMeshProUGUI platScoreTresholdText;

    [SerializeField] private GameObject whiteSheepGO;
    [SerializeField] private GameObject blackSheepGO;
    [SerializeField] private GameObject blueSheepGO;
    [SerializeField] private GameObject greenSheepGO;
    [SerializeField] private GameObject redSheepGO;
    [SerializeField] private GameObject goatSheepGO;
    [SerializeField] private GameObject goldSheepGO;

    [SerializeField] private TextMeshProUGUI whiteSheepNumber;
    [SerializeField] private TextMeshProUGUI blackSheepNumber;
    [SerializeField] private TextMeshProUGUI blueSheepNumber;
    [SerializeField] private TextMeshProUGUI greenSheepNumber;
    [SerializeField] private TextMeshProUGUI redSheepNumber;
    [SerializeField] private TextMeshProUGUI goldSheepNumber;
    [SerializeField] private TextMeshProUGUI goatSheepNumber;


    private int playerTrophies;
    private int timeScore;
    private int levelTime;

    private int platScoreTreshold;
    private int goldScoreTreshold;
    private int silverScoreTreshold;
    private int bronzeScoreTreshold;


    [SerializeField] private GameObject bronzeTrophyIcon;
    [SerializeField] private GameObject silverTrophyIcon;
    [SerializeField] private GameObject goldTrophyIcon;
    [SerializeField] private GameObject platTrophyIcon;

    private int initialWhiteSheepNumber;
    private int initialBlackSheepNumber;
    private int initialRedSheepNumber;
    private int initialGreenSheepNumber;
    private int initialBlueSheepNumber;
    private int initialGoldSheepNumber;
    private int initialGoatSheepNumber;
    #endregion

    private bool cinematicPlaying;

    private void Awake() {
        levelDisplayUI.SetActive(false);
        levelUnlockUI.SetActive(false);
    }

    private void Start() {
        levelSO = questGiver.GetLevelSO();

        questGiver.OnLevelBoneFeePaid += QuestGiver_OnLevelBoneFeePaid;
        GameInput.Instance.OnBarkPressed += GameInput_OnBarkPressed;
        GameInput.Instance.OnBarkReleased += Instance_OnBarkReleased;
        GameInput.Instance.OnExitPerformed += GameInput_OnExitPerformed;

        whiteSheepScore = whiteSheepSO.sheepScore;
        blackSheepScore = blackSheepSO.sheepScore;
        blueSheepScore = blueSheepSO.sheepScore;
        redSheepScore = redSheepSO.sheepScore;
        goldSheepScore = goldSheepSO.sheepScore;
        greenSheepScore = greenSheepSO.sheepScore;
        goatSheepScore = goatSheepSO.sheepScore;

        initialWhiteSheepNumber = levelSO.whiteSheepNumber;
        initialBlackSheepNumber =(levelSO.blackSheepNumber);
        initialBlueSheepNumber=(levelSO.blueSheepNumber);
        initialGoatSheepNumber = levelSO.goatSheepNumber;
        initialGoldSheepNumber = levelSO.goldSheepNumber;
        initialGreenSheepNumber = levelSO.greenSheepNumber;
        initialRedSheepNumber = levelSO.redSheepNumber;

        timeScore = levelSO.timeScore;
        levelTime = levelSO.levelTimeLimit;

        // Initialise score thresholds
        InitializeScoreTresholds();

        DisplayLevelParameters();
    }

    private void InitializeScoreTresholds() {
        if (timeScore < 5) {
            // Sheep based level
            float maxSheepScore = initialWhiteSheepNumber * whiteSheepScore +
                initialBlackSheepNumber * blackSheepScore +
                initialRedSheepNumber * redSheepScore +
                initialGreenSheepNumber * greenSheepScore +
                initialBlueSheepNumber * blueSheepScore +
                initialGoldSheepNumber * goldSheepScore +
                initialGoatSheepNumber * goatSheepScore;

            platScoreTreshold = ((int)((maxSheepScore + levelTime * timeScore / 2) / 10)) * 10;
            goldScoreTreshold = ((int)((9 * platScoreTreshold / 10) / 10)) * 10;
            silverScoreTreshold = ((int)((4 * platScoreTreshold / 5) / 10)) * 10;
            bronzeScoreTreshold = ((int)((7 * platScoreTreshold / 10) / 10)) * 10;

        }
        else {
            // Time based level

            float maxSheepScore = initialWhiteSheepNumber * whiteSheepScore +
                initialBlackSheepNumber * blackSheepScore +
                initialRedSheepNumber * redSheepScore +
                initialGreenSheepNumber * greenSheepScore +
                initialBlueSheepNumber * blueSheepScore +
                initialGoldSheepNumber * goldSheepScore +
                initialGoatSheepNumber * goatSheepScore;

            float maxTimeScore = levelTime * timeScore;

            platScoreTreshold = ((int)((maxSheepScore / 2 + 3 * maxTimeScore / 4) / 10)) * 10;
            goldScoreTreshold = ((int)((9 * platScoreTreshold / 10) / 10)) * 10;
            silverScoreTreshold = ((int)((4 * platScoreTreshold / 5) / 10)) * 10;
            bronzeScoreTreshold = ((int)((7 * platScoreTreshold / 10) / 10)) * 10;
        }

    }

    private void Instance_OnBarkReleased(object sender, System.EventArgs e) {
        if (playerIsInTriggerZone & !cinematicPlaying) {
            DisplayLevelUI();
            Player.Instance.GetComponent<PlayerMovement>().SetCanMove(false);
        }
    }

    private void GameInput_OnExitPerformed(object sender, System.EventArgs e) {
        if (playerIsInTriggerZone & !cinematicPlaying) {
            levelDisplayUI.SetActive(false);
            levelUnlockUI.SetActive(false);
            Player.Instance.GetComponent<PlayerMovement>().SetCanMove(true);
        }
    }

    private void GameInput_OnBarkPressed(object sender, System.EventArgs e) {
        
    }

    public void DisplayLevelUI() {
        if (questGiver.GetQuestBoneFeePaid()) {
            levelDisplayUI.SetActive(true);
        }
        else {
            levelUnlockUI.SetActive(true);
        }
    }

    private void QuestGiver_OnLevelBoneFeePaid(object sender, System.EventArgs e) {
        UnlockLevelUI();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.TryGetComponent<Player>(out Player player)) {
            playerIsInTriggerZone = true;
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent<Player>(out Player player)) {
            playerIsInTriggerZone = false;
            levelDisplayUI.SetActive(false);
            levelUnlockUI.SetActive(false);
        }
    }

    public void UnlockLevelUI() {
        if (Player.Instance.GetPlayerBones() >= levelSO.levelBoneUnlockCost) {
            levelUnlockUI.SetActive(false);
            levelDisplayUI.SetActive(true);
        }
    }

    private void DisplayLevelParameters() {
        levelNameLocked.text = levelSO.levelName;
        levelNameUnlocked.text = levelSO.levelName;
        bonePriceText.text = levelSO.levelBoneUnlockCost.ToString();
        initialTime.text = levelSO.levelTimeLimit.ToString();


        bronzeScoreTresholdText.text = "+" + bronzeScoreTreshold.ToString();
        silverScoreTresholdText.text = "+" + silverScoreTreshold.ToString();
        goldScoreTresholdText.text = "+" + goldScoreTreshold.ToString();
        platScoreTresholdText.text = "+" + platScoreTreshold.ToString();

        bronzeTrophyIcon.SetActive(false);
        silverTrophyIcon.SetActive(false);
        goldTrophyIcon.SetActive(false);
        platTrophyIcon.SetActive(false);

        if (questGiver.GetQuestCompleted()) {
            highScore.text = ES3.Load(levelSO.levelName + "_highScore", 0).ToString();

            playerTrophies = ES3.Load(levelSO.levelName + "_highTrophies", 0);
            if (playerTrophies == 1) {
                bronzeTrophyIcon.SetActive(true);
            }
            if (playerTrophies == 2) {
                silverTrophyIcon.SetActive(true);
            }
            if (playerTrophies == 3) {
                goldTrophyIcon.SetActive(true);
            }
            if (playerTrophies == 4) {
                platTrophyIcon.SetActive(true);
            }
        } else {
            highScore.text = "-";
        }

        whiteSheepGO.SetActive(false);
        goldSheepGO.SetActive(false);
        blueSheepGO.SetActive(false);
        redSheepGO.SetActive(false);
        greenSheepGO.SetActive(false);
        blackSheepGO.SetActive(false);
        goatSheepGO.SetActive(false);


        if (initialWhiteSheepNumber != 0) {
            whiteSheepGO.SetActive(true);
        }
        if(initialRedSheepNumber != 0) {
            redSheepGO.SetActive(true);
        }
        if(initialBlueSheepNumber != 0) {
            blueSheepGO.SetActive(true);
        }
        if(initialGreenSheepNumber != 0) {
            greenSheepGO.SetActive(true);
        }
        if(initialGoldSheepNumber != 0) {
            goldSheepGO.SetActive(true);
        }
        if(initialBlackSheepNumber != 0) {
            blackSheepGO.SetActive(true);
        }
        if(initialGoatSheepNumber != 0) {
            goatSheepGO.SetActive(true);
        }

        whiteSheepNumber.text = initialWhiteSheepNumber.ToString();
        blackSheepNumber.text = initialBlackSheepNumber.ToString();
        blueSheepNumber.text = initialBlueSheepNumber.ToString();
        redSheepNumber.text = initialRedSheepNumber.ToString();
        greenSheepNumber.text = initialGreenSheepNumber.ToString();
        goldSheepNumber.text = initialGoldSheepNumber.ToString();
        goatSheepNumber.text = initialGoatSheepNumber.ToString();


    }

    public void SetCinematicPlaying(bool cinematicPlaying) {
        this.cinematicPlaying = cinematicPlaying;
    }

}
