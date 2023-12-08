using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] Button startGameButton;

    [SerializeField] Image backGroundImage;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject resetProgressionPanel;
    [SerializeField] GameObject progressionPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject resetProgressionButton;

    [SerializeField] TextMeshProUGUI playerProgressionText;
    [SerializeField] TextMeshProUGUI boneNumberText;
    [SerializeField] TextMeshProUGUI bronzeBoneNumberText;
    [SerializeField] TextMeshProUGUI silverBoneNumberText;
    [SerializeField] TextMeshProUGUI goldBoneNumberText;
    [SerializeField] TextMeshProUGUI platBoneNumberText;

    float boneNumber;
    float bronzeBoneNumber;
    float silverBoneNumber;
    float goldBoneNumber;
    float platBoneNumber;
    float levelCompletedNumber;

    private void Start() {

        resetProgressionPanel.SetActive(false);
        resetProgressionButton.SetActive(false);

        AssignImageParametersByAspectRatio();
        LoadPlayerProgression();

        if (levelCompletedNumber == 0) {
            startGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
            progressionPanel.SetActive(false);
        }
        else {
            resetProgressionButton.SetActive(true);
            startGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
        }

        DisplayPlayerProgression();
    }

    private void StartGameButtonScript_OnButtonSelected(object sender, System.EventArgs e) {
    }

    private void StartGameButtonScript_OnButtonDeSelected(object sender, System.EventArgs e) {
    }

    private void AssignImageParametersByAspectRatio() {
        if (Camera.main.aspect >= 1.7) {
            backGroundImage.transform.position = Vector3.zero;
            backGroundImage.transform.localScale = Vector3.one * 2f;
            backGroundImage.GetComponent<Animator>().SetBool("PC", true);

            mainMenuPanel.transform.localPosition = new Vector3(-500f, -100f, 0);
            mainMenuPanel.transform.localScale = Vector3.one;
            Debug.Log("16:9");
        }
        else if (Camera.main.aspect >= 1.5) {
            Debug.Log("3:2");
        }
        else {
            backGroundImage.transform.position = Vector3.zero;
            backGroundImage.transform.localScale = Vector3.one * 2f;
            backGroundImage.GetComponent<Animator>().SetBool("Phone", true);

            mainMenuPanel.transform.localPosition = new Vector3(0, -150, 0);
            mainMenuPanel.transform.localScale = Vector3.one * 1.25f;
            Debug.Log("9:16");
        }
    }

    private void LoadPlayerProgression() {
        boneNumber = ES3.Load("playerBones", 0);
        bronzeBoneNumber = GetBoneNumber("bronzeBone");
        silverBoneNumber = GetBoneNumber("silverBone");
        goldBoneNumber = GetBoneNumber("goldBone");
        platBoneNumber = GetBoneNumber("platBone"); ;
        levelCompletedNumber = GetLevelCompletedNumber();
    }

    private void DisplayPlayerProgression() {
        playerProgressionText.text = levelCompletedNumber.ToString() + "/" + GetLevelNumber();
        boneNumberText.text = boneNumber.ToString();
        bronzeBoneNumberText.text = bronzeBoneNumber.ToString();
        silverBoneNumberText.text = silverBoneNumber.ToString();
        goldBoneNumberText.text = goldBoneNumber.ToString();
        platBoneNumberText.text = platBoneNumber.ToString();
    }

    private int GetLevelNumber() {
        int levelNumber = LevelSOList.Instance.GetlevelSOList().Count;
        return levelNumber;
    }

    private int GetLevelCompletedNumber() {
        int levelCompletedNumber = 0;

        List<LevelSO> levelSOList = new List<LevelSO>();
        levelSOList = LevelSOList.Instance.GetlevelSOList();

        foreach (LevelSO levelSO in levelSOList) {
            string levelID = levelSO.levelName + "_completed";
            if (ES3.Load(levelID, false)) {
                // bone type is unlocked
                levelCompletedNumber++;
            }
        }

        return levelCompletedNumber;
    }

    private int GetBoneNumber(string boneType) {
        int boneNumber = 0;

        List<LevelSO> levelSOList = new List<LevelSO>();
        levelSOList = LevelSOList.Instance.GetlevelSOList();

        foreach(LevelSO levelSO in levelSOList) {
            string boneID = levelSO.levelName + "_" + boneType;
            if(ES3.Load(boneID, false)) {
                // bone type is unlocked
                boneNumber++;
            }
        }

        return boneNumber;
    }

    public void OpenResetProgressionPanel() {
        resetProgressionPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseResetProgressionPanel() {
        resetProgressionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ResetProgression() {
        ES3.DeleteFile();
        startGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
        progressionPanel.SetActive(false);
        resetProgressionButton.SetActive(false);

        resetProgressionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
