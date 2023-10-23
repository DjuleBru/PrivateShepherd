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
    [SerializeField] GameObject progressionPanel;
    [SerializeField] GameObject settingsPanel;

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
        AssignImageParametersByAspectRatio();
        LoadPlayerProgression();

        if (levelCompletedNumber == 0) {
            startGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
            progressionPanel.SetActive(false);
        }
        else {
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
        bronzeBoneNumber = ES3.Load("bronzeBones", 0);
        silverBoneNumber = ES3.Load("silverBones", 0);
        goldBoneNumber = ES3.Load("goldBones", 0);
        platBoneNumber = ES3.Load("platBones", 0);
        levelCompletedNumber = ES3.Load("levelCompletedNumber", 0);
    }

    private void DisplayPlayerProgression() {
        playerProgressionText.text = levelCompletedNumber.ToString() + "/10";
        boneNumberText.text = boneNumber.ToString();
        bronzeBoneNumberText.text = bronzeBoneNumber.ToString();
        silverBoneNumberText.text = silverBoneNumber.ToString();
        goldBoneNumberText.text = goldBoneNumber.ToString();
        platBoneNumberText.text = platBoneNumber.ToString();
    }
}
