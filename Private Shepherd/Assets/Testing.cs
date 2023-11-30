using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] QuestGiver testQuestGiver;
    [SerializeField] QuestGiver testQuestGiver2;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.T)) {
            Player.Instance.GivePlayerBones(1);
        };
    }
}
