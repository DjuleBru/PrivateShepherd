using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapLevelDisplay : MonoBehaviour
{
    [SerializeField] private Collider2D collider;
    [SerializeField] private LevelSO levelSO;
    [SerializeField] private GameObject levelDisplayUI;
    [SerializeField] private GameObject levelUnlockUI;

    #region LEVEL PARAMETERS

    private int levelSOCount = 0;

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

    private bool levelUnlocked;
    private bool levelCompleted;

    private void Awake() {
        levelDisplayUI.SetActive(false);
        levelUnlockUI.SetActive(false);

    }

    private void Start() {
        LoadLevelParameters();
        DisplayLevelParameters();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.TryGetComponent<Player>(out Player player)) {
            if (levelUnlocked) {
                levelDisplayUI.SetActive(true);
            } else {
                levelUnlockUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent<Player>(out Player player)) {
            levelDisplayUI.SetActive(false);
            levelUnlockUI.SetActive(false);
        }
    }

    public void UnlockLevel() {
        if (Player.Instance.GetPlayerBones() >= levelSO.levelBoneUnlockCost) {

            Player.Instance.SpendPlayerBones(levelSO.levelBoneUnlockCost);

            levelUnlockUI.SetActive(false);
            levelDisplayUI.SetActive(true);

            levelUnlocked = true;
            ES3.Save(levelSO.levelName + "_unlocked", levelUnlocked);
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

        if (levelCompleted) {
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

    private void LoadLevelParameters() {
        levelCompleted = ES3.Load(levelSO.levelName + "_completed", false);
        levelUnlocked = ES3.Load(levelSO.levelName + "_unlocked", false);
    }

}
