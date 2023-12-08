using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public static TutorialManager Instance { get; private set; }

    [SerializeField] SheepObjectPool levelSheepObjectPool;
    private List<Sheep> sheepList;

    private bool sheepFleedOnce;
    private bool splitHerdTutorialPhase;
    private bool splitHerdTutorialPhaseDone;
    private bool tutorialFinished;

    private SheepMovement firstSheepToFlee;
    private Sheep sheepSplitHerd;

    private float sheepIsolatedTimer;
    [SerializeField] private float sheepIsolatedTime;

    private int sheepCount;
    [SerializeField] private Transform penPosition;
    private void Awake() {
        Instance = this;
    }

    void Start()
    {
        sheepIsolatedTimer = sheepIsolatedTime;
        sheepList = levelSheepObjectPool.GetSheepsInObjectPoolList();

        foreach(Sheep sheep in sheepList) {
            sheep.GetComponent<SheepMovement>().OnSheepFlee += Sheep_OnSheepFlee;
        }
    }

    private void Update() {
        sheepCount = 0;
        foreach (Sheep sheep in sheepList) {
            float herdNumber = sheep.GetComponentInChildren<SheepHerd>().GetHerdNumber();

            if(herdNumber < 3 & splitHerdTutorialPhase & !splitHerdTutorialPhaseDone) {
                sheepIsolatedTimer -= Time.deltaTime;

                if(sheepIsolatedTimer < 0 ) {
                    sheepSplitHerd = sheep;
                    splitHerdTutorialPhaseDone = true;
                    FreezeSheep();

                    LevelIntroNPC.Instance.SetInputActive(true);
                    LevelIntroCutScene.Instance.EnterCutScene();
                    LevelIntroCutScene.Instance.SetCameraToNPC();
                    LevelIntroNPC.Instance.StartTalking();
                    LevelIntroNPC.Instance.NextPhrase();

                    LevelIntroCutScene.Instance.AddCameraPosition(sheepSplitHerd.transform);
                    return;
                }
            }

            if(herdNumber == 3 | herdNumber == 4 & splitHerdTutorialPhaseDone) {
                sheepCount++;
            } else {
                sheepCount = 0;
            }
        }

        if (splitHerdTutorialPhaseDone & sheepCount == sheepList.Count & !tutorialFinished) {
            FreezeSheep();
            tutorialFinished = true;

            LevelIntroNPC.Instance.SetInputActive(true);
            LevelIntroCutScene.Instance.EnterCutScene();
            LevelIntroCutScene.Instance.SetCameraToNPC();
            LevelIntroNPC.Instance.StartTalking();
            LevelIntroNPC.Instance.NextPhrase();

            LevelIntroCutScene.Instance.AddCameraPosition(penPosition);
            return;
        }
    }

    private void Sheep_OnSheepFlee(object sender, System.EventArgs e) {
        firstSheepToFlee = (SheepMovement)sender;
        if (!sheepFleedOnce) {
            sheepFleedOnce = true;
            StartCoroutine(OnSheepFleedOnce());
        }
    }

    private IEnumerator OnSheepFleedOnce() {
        yield return new WaitForSeconds(1f);

        LevelIntroNPC.Instance.SetInputActive(true);
        LevelIntroCutScene.Instance.EnterCutScene();
        LevelIntroCutScene.Instance.SetCameraToNPC();
        LevelIntroNPC.Instance.StartTalking();
        LevelIntroNPC.Instance.NextPhrase();

        FreezeSheep();
        LevelIntroCutScene.Instance.AddCameraPosition(firstSheepToFlee.transform);
    }

    public void FreezeSheep() {
        foreach (Sheep sheep in sheepList) {
            sheep.GetComponent<SheepMovement>().SetMovementTimeScale(0f);
            sheep.GetComponent<SetAIAnimatorParameters>().SetAnimatorTimeScale(0f);
        }
        splitHerdTutorialPhase = true;
    }

    public void PlaySheep() {
        foreach (Sheep sheep in sheepList) {
            sheep.GetComponent<SheepMovement>().SetMovementTimeScale(1f);
            sheep.GetComponent<SetAIAnimatorParameters>().SetAnimatorTimeScale(1f);
        }
    }

    public void EndTutorial() {
        ES3.Save("Tutorial_completed", true);
    }
}
