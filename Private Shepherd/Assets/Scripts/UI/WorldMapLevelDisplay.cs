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

    #region LEVEL PARAMETERS

    private int levelSOCount = 0;

    private LevelSO levelSO;
    [SerializeField] private TextMeshProUGUI levelNameLocked;
    [SerializeField] private TextMeshProUGUI levelNameUnlocked;
    [SerializeField] private TextMeshProUGUI highScore;
    [SerializeField] private TextMeshProUGUI bonePriceText;
    [SerializeField] private TextMeshProUGUI initialTime;
    [SerializeField] private TextMeshProUGUI sheepTotalNumber;

    [SerializeField] private TextMeshProUGUI bronzeScoreTreshold;
    [SerializeField] private TextMeshProUGUI silverScoreTreshold;
    [SerializeField] private TextMeshProUGUI goldScoreTreshold;
    [SerializeField] private TextMeshProUGUI platScoreTreshold;

    private int playerTrophies;

    [SerializeField] private GameObject bronzeTrophyIcon;
    [SerializeField] private GameObject silverTrophyIcon;
    [SerializeField] private GameObject goldTrophyIcon;
    [SerializeField] private GameObject platTrophyIcon;
    #endregion

    private void Awake() {
        levelDisplayUI.SetActive(false);
        levelUnlockUI.SetActive(false);
    }

    private void Start() {
        levelSO = questGiver.GetLevelSO();
        DisplayLevelParameters();

        questGiver.OnLevelBoneFeePaid += QuestGiver_OnLevelBoneFeePaid;
        GameInput.Instance.OnBarkPressed += GameInput_OnBarkPressed;
        GameInput.Instance.OnBarkReleased += Instance_OnBarkReleased;
        GameInput.Instance.OnExitPerformed += GameInput_OnExitPerformed;
    }

    private void Instance_OnBarkReleased(object sender, System.EventArgs e) {
        if (playerIsInTriggerZone) {
            DisplayLevelUI();
            Player.Instance.GetComponent<PlayerMovement>().SetCanMove(false);
        }
    }

    private void GameInput_OnExitPerformed(object sender, System.EventArgs e) {
        if (playerIsInTriggerZone) {
            levelDisplayUI.SetActive(false);
        }
        Player.Instance.GetComponent<PlayerMovement>().SetCanMove(true);
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
        sheepTotalNumber.text = levelSO.sheepNumber.ToString();
        bronzeScoreTreshold.text = "+" + levelSO.bronzeScoreTreshold.ToString();
        silverScoreTreshold.text = "+" + levelSO.silverScoreTreshold.ToString();
        goldScoreTreshold.text = "+" + levelSO.goldScoreTreshold.ToString();
        platScoreTreshold.text = "+" + levelSO.platScoreTreshold.ToString();

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

    }

}
