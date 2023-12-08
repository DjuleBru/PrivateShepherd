using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using Febucci.UI;
using System;

public class LevelTutorialNPC : MonoBehaviour
{

    public static LevelTutorialNPC Instance { get; private set; }

    public List<string> phrases = new List<string>();
    [SerializeField] private TextMeshProUGUI phraseText;

    private int phraseCount;
    private float defaultCameraSpeed = 8f;

    [SerializeField] TypewriterByCharacter typeWriter;
    [SerializeField] float[] cameraZooms;
    [SerializeField] float[] cameraSpeeds;
    int i;

    private bool inputActiveForTutorial;
    private bool textShowed;
    private bool introFinished;

    public event EventHandler OnTutorialPaused;

    private void Awake() {
        Instance = this;
        phraseText.gameObject.SetActive(false);
    }

    private void Update() {
        if (!inputActiveForTutorial) {
            return;
        }
        if (Input.anyKeyDown) {
            if (!textShowed) {
                typeWriter.SkipTypewriter();
            }
            else {
                NextPhrase();
            }
        }
    }

    public void StartTalking() {
        phraseText.gameObject.SetActive(true);
        phraseCount = 0;
        typeWriter.ShowText(phrases[phraseCount]);

        inputActiveForTutorial = true;
    }

    public void StopTalking() {
        inputActiveForTutorial = false;
        if (phraseText != null) {
            phraseText.gameObject.SetActive(false);
            LevelIntroCutScene.Instance.SetNPCIsTalking(false);
        }
        introFinished = true;
    }

    public void PauseTalking() {
        inputActiveForTutorial = false;
        if (phraseText != null) {
            phraseText.gameObject.SetActive(false);
            LevelIntroCutScene.Instance.SetNPCIsTalking(false);
        }
        introFinished = true;
    }

    public void NextPhrase() {
        phraseCount++;
        if (LevelIntroCutScene.Instance.GetCameraLocked()) {
            LevelIntroCutScene.Instance.SetCameraLocked(false);
        }
        if (phraseCount >= phrases.Count) {
            StopTalking();
            return;
        }
        if (phrases[phraseCount] == "camera") {
            float zoom = cameraZooms[i];
            float cameraSpeed = defaultCameraSpeed;
            if (cameraSpeeds[i] != 0) {
                cameraSpeed = cameraSpeeds[i];
            }
            i++;
            LevelIntroCutScene.Instance.MoveCameraToNextPosition(cameraSpeed, zoom);
            return;
        }
        if (phrases[phraseCount] == "cameraLock") {
            float zoom = cameraZooms[i];
            float cameraSpeed = defaultCameraSpeed;
            if (cameraSpeeds[i] != 0) {
                cameraSpeed = cameraSpeeds[i];
            }
            i++;
            LevelIntroCutScene.Instance.LockCameraToNextPosition(cameraSpeed, zoom);
            return;
        }
        if (phrases[phraseCount] == "pauseTutorial") {
            i++;
            PauseTalking();
            OnTutorialPaused?.Invoke(this, EventArgs.Empty);
            return;
        }
        if (phrases[phraseCount] == "resetCamera") {
            LevelIntroCutScene.Instance.SetCameraToNPC();
            return;
        }
        typeWriter.ShowText(phrases[phraseCount]);
    }

    public void SetTextShowed(bool showed) {
        textShowed = showed;
    }

    public void SetInputActive(bool active) {
        inputActiveForTutorial = active;
    }

    public bool GetInputActiveForIntro() {
        return inputActiveForTutorial;
    }

    public bool GetTextShowed() {
        return textShowed;
    }

    public bool GetIntroFinished() {
        return introFinished;
    }
}
