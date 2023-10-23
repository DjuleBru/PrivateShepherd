using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiverGroup : MonoBehaviour
{
    QuestGiver[] questGiversInGroup;
    List<QuestGiver> unLockedQuestGiversInGroup;

    int activeQuestGiver;
    int totalQuestGiversInGroup;

    private void Awake() {
        questGiversInGroup = GetComponentsInChildren<QuestGiver>();

        FillQuestGiversUnlockedList();
        DisableAllQuestGivers();
    }

    void Start()
    {
        if(totalQuestGiversInGroup >= 1) {
            activeQuestGiver = totalQuestGiversInGroup - 1;
            unLockedQuestGiversInGroup[activeQuestGiver].gameObject.SetActive(true);
        }
    }

    public void NextQuestGiver() {
        if(activeQuestGiver < totalQuestGiversInGroup - 1) {
            unLockedQuestGiversInGroup[activeQuestGiver +1].gameObject.SetActive(true);
            unLockedQuestGiversInGroup[activeQuestGiver + 1].DisplayWorldMapLevelDisplay();
            unLockedQuestGiversInGroup[activeQuestGiver].gameObject.SetActive(false);
            activeQuestGiver++;
        }

    }

    public void PreviousQuestGiver() {
        if (activeQuestGiver > 0) {
            unLockedQuestGiversInGroup[activeQuestGiver - 1].gameObject.SetActive(true);
            unLockedQuestGiversInGroup[activeQuestGiver - 1].DisplayWorldMapLevelDisplay();
            unLockedQuestGiversInGroup[activeQuestGiver].gameObject.SetActive(false);
            activeQuestGiver--;
        }
    }

    private void DisableAllQuestGivers() {
        foreach(QuestGiver questGiver in questGiversInGroup) {
            questGiver.gameObject.SetActive(false);
        }
    }

    private void FillQuestGiversUnlockedList() {
        unLockedQuestGiversInGroup = new List<QuestGiver>();

        foreach (QuestGiver questGiver in questGiversInGroup) {
            if(questGiver.GetQuestGiverUnlocked()) {
                // QuestGiver is unlocked
                unLockedQuestGiversInGroup.Add(questGiver);
            }
        }

        totalQuestGiversInGroup = unLockedQuestGiversInGroup.Count;
    }

    public List<QuestGiver> GetQuestGiversUnlockedList() {
        return unLockedQuestGiversInGroup;
    }
}
