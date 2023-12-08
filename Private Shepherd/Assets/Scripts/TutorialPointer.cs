using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPointer : MonoBehaviour
{

    public enum Target {
        player,
        sheep
    }

    [SerializeField] Target target;
    [SerializeField] GameObject targetVisual;

    [SerializeField] private int tutorialNumber;
    private int tutorialCount;

    private bool targetActive;
    private bool playerEnteredZone;
    private bool tutorialPhaseEnded;

    [SerializeField] private bool tutorialEndPointed;

    private void Start() {
        LevelIntroNPC.Instance.OnTutorialPaused += Instance_OnTutorialPaused;
        targetVisual.SetActive(false);
    }

    private void Update() {

        if(playerEnteredZone) {
            float distancePlayerVSNPC = Vector3.Distance(LevelIntroNPC.Instance.transform.position, Player.Instance.transform.position);
            float treshold = 3f;

            if(distancePlayerVSNPC < treshold & !tutorialPhaseEnded) {
                tutorialPhaseEnded = true;

                LevelIntroNPC.Instance.SetInputActive(true);
                SetTargetActive(false);
                LevelIntroCutScene.Instance.EnterCutScene();
                LevelIntroCutScene.Instance.SetCameraToNPC();
                LevelIntroNPC.Instance.StartTalking();
                LevelIntroNPC.Instance.NextPhrase();
            }
        }

    }

    private void Instance_OnTutorialPaused(object sender, System.EventArgs e) {
        tutorialCount++;
        if(tutorialCount == tutorialNumber) {
            SetTargetActive(true);
        }
        if(tutorialCount == tutorialNumber & tutorialEndPointed) {
            SetTargetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (targetActive & collision.gameObject == Player.Instance.gameObject & target == Target.player) {
            playerEnteredZone = true;
        }
    }

    public void SetTargetActive(bool active) {
        targetActive = active;
        targetVisual.SetActive(active);
        gameObject.SetActive(active);
    }

}
