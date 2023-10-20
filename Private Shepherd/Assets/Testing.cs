using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] QuestGiver testQuestGiver;
    [SerializeField] QuestGiver testQuestGiver2;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            testQuestGiver.completeQuestTest();
            testQuestGiver.TestFunction();
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            testQuestGiver2.completeQuestTest();
            testQuestGiver2.TestFunction();
        }
    }
}
