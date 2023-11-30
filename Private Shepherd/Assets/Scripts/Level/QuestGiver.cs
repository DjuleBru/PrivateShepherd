using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] QuestGiver[] questGiverUnlocks;
    [SerializeField] private bool isFirstQuestGiver;
    QuestGiverGroup questGiverParent;

    [SerializeField] WorldMapLevelDisplay mapLevelDisplay;
    [SerializeField] GameObject questGiverVisual;
    [SerializeField] GameObject questGiverUnlockedIcon;
    [SerializeField] Transform questUnlockCameraFollowTransform;
    [SerializeField] LevelSO levelSO;

    public event EventHandler OnLevelBoneFeePaid;

    private bool questGiverUnlocked;
    private bool questBoneFeePaid;
    private bool questCompleted;
    private bool linkedQuestGiversUnlocked;
    private int questGiverUnlockedInThisGroupNumber;

    private float cinemachineQuestUnlockDezoom = 18f;
    private float cinemachineQuestUnlockZoom = 10f;

    private void Awake() {
        LoadLevelParameters();
        questGiverParent = GetComponentInParent<QuestGiverGroup>();

        if (isFirstQuestGiver) {
            questGiverUnlocked = true;
            ES3.Save(levelSO.levelName + "_unlocked", questGiverUnlocked);
            questGiverUnlockedIcon.SetActive(true);
        }
    }

    private void Start() {
        questGiverUnlockedIcon.SetActive(false);
        if (questGiverUnlocked & !questCompleted) {
            questGiverUnlockedIcon.SetActive(true);
        }

        questGiverUnlockedInThisGroupNumber = questGiverParent.GetQuestGiversUnlockedList().Count;
        if (!questGiverUnlocked & questGiverUnlockedInThisGroupNumber == 0) {
            // There are no other questGivers Unlocked in this category AND QuestGiver is locked
            questGiverVisual.SetActive(false);
        }

    }

    private void Update() {
        if (questCompleted & !linkedQuestGiversUnlocked) {
            StartCoroutine(UnlockLinkedQuestGivers());

            linkedQuestGiversUnlocked = true;
            ES3.Save(levelSO.levelName + "_linkedQuestUnlocked", linkedQuestGiversUnlocked);
        }
    }
         
    private void LoadLevelParameters() {
        questCompleted = ES3.Load(levelSO.levelName + "_completed", false);
        questBoneFeePaid = ES3.Load(levelSO.levelName + "_boneFeePaid", false);
        questGiverUnlocked = ES3.Load(levelSO.levelName + "_unlocked", false);
        linkedQuestGiversUnlocked = ES3.Load(levelSO.levelName + "_linkedQuestUnlocked", false);
    }


    public void PayBoneFee() {
        if (Player.Instance.GetPlayerBones() >= levelSO.levelBoneUnlockCost) {

            Player.Instance.SpendPlayerBones(levelSO.levelBoneUnlockCost);

            OnLevelBoneFeePaid?.Invoke(this, EventArgs.Empty);

            questBoneFeePaid = true;
            ES3.Save(levelSO.levelName + "_boneFeePaid", questBoneFeePaid);
        }
    }

    public IEnumerator UnlockLinkedQuestGivers() {

        // DeActivate player movement
        Player.Instance.gameObject.GetComponent<PlayerMovement>().SetCanMove(false);

        // Start by dezooming
        float initialOrthoSize = ChangeCinemachineCode.Instance.GetCinemachineOrthoSize();
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(cinemachineQuestUnlockDezoom);
        yield return new WaitForSeconds(.5f);

        int questGiverIndex = 0;
        int questGiverMaxIndex = questGiverUnlocks.Length - 1;

        while (questGiverIndex <= questGiverMaxIndex) {
            QuestGiver questGiver = questGiverUnlocks[questGiverIndex];
            questGiver.gameObject.SetActive(true);

            if (questGiverIndex == 0) {
                // Sends the instruction to other questGivers to unlock with the player
                questGiver.UnlockQuestGiver(Player.Instance.transform);
            } else {
                QuestGiver previousQuestGiver = questGiverUnlocks[questGiverIndex - 1];
                // Sends the instruction to other questGivers to unlock with the previous questGiver as origin
                questGiver.UnlockQuestGiver(previousQuestGiver.transform);
            }

            while(!questGiver.GetQuestGiverUnlocked()) {
                // Wait until questGiver is unlocked
                yield return null;
            }

            questGiverIndex++;
        }

        // Back camera to player
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(Player.Instance.transform);
        ChangeCinemachineCode.Instance.ChangeCinemachineOrthoSize(initialOrthoSize);

        // ReActivate player movement
        Player.Instance.gameObject.GetComponent<PlayerMovement>().SetCanMove(true);

        yield return null;
    }

    public void UnlockQuestGiver(Transform questUnlockerOrigin) {
        StartCoroutine(UnlockQuestGiverCoroutine(questUnlockerOrigin));
    }

    public IEnumerator UnlockQuestGiverCoroutine(Transform questUnlockerOrigin) {

        // Initialize camera transform and dezoom
        questUnlockCameraFollowTransform.transform.position = questUnlockerOrigin.position;
        ChangeCinemachineCode.Instance.ChangeCinemachineTarget(questUnlockCameraFollowTransform);

        // Smooth camera movement towards quest giver
        Vector3 destination = this.transform.position;
        float treshold = .2f;
        float cameraSpeed = .2f;
        Vector3 directionNormalized = (destination - questUnlockerOrigin.position).normalized;

        while ((questUnlockCameraFollowTransform.position - destination).magnitude > treshold) {
            questUnlockCameraFollowTransform.position += directionNormalized * cameraSpeed;
            yield return null;
        }
        questUnlockCameraFollowTransform.position = destination;

        // Camera zoom
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(cinemachineQuestUnlockZoom);
        yield return new WaitForSeconds(.5f);

        if (questGiverParent.GetQuestGiversUnlockedList().Count == 0) {
            // This is the first questGiver in its category to unlock
            questGiverVisual.SetActive(true);
            questGiverVisual.GetComponent<QuestGiverVisual>().SetAnimatorTrigger();
        }

        yield return new WaitForSeconds(1f);
        // Unlock Icon
        questGiverUnlockedIcon.SetActive(true);

        yield return new WaitForSeconds(.5f);

        // Camera dezoom
        ChangeCinemachineCode.Instance.SmoothCinemachineZoom(cinemachineQuestUnlockDezoom);
        yield return new WaitForSeconds(1f);

        questGiverUnlocked = true;
        ES3.Save(levelSO.levelName + "_unlocked", questGiverUnlocked);
        yield return null;
    }


    public bool GetQuestGiverUnlocked() {
        return questGiverUnlocked;
    }

    public void NextQuestGiver() {
        questGiverParent.NextQuestGiver();
    }

    public void PreviousQuestGiver() {
        questGiverParent.PreviousQuestGiver();
    }

    public bool GetQuestBoneFeePaid() {
        return questBoneFeePaid;
    }

    public bool GetQuestCompleted() {
        return questCompleted;
    }

    public LevelSO GetLevelSO() {
        return levelSO;
    }

    public int GetQuestGiverUnlockedInThisGroupNumber() {
        return questGiverUnlockedInThisGroupNumber;
    }

    public void DisplayWorldMapLevelDisplay() {
        mapLevelDisplay.DisplayLevelUI();
    }

}
