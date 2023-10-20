using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiverVisual : MonoBehaviour
{
    private Animator animator;
    private QuestGiver questGiver;
    [SerializeField] private MMF_Player player;

    private bool hasPlayedFeedbacks;

    private void Start() {
        animator = GetComponent<Animator>();
        questGiver = GetComponentInParent<QuestGiver>();
    }

    private void Update() {
        if (hasPlayedFeedbacks | questGiver.GetQuestGiverUnlockedInThisGroupNumber() != 0) {
            // Has already played feedbacks OR there are already questgivers unlocked in this group
            return;
        }

        if(!AnimatorIsPlaying()) {
            player.PlayFeedbacks();
            hasPlayedFeedbacks = true;
        }
    }


    bool AnimatorIsPlaying() {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void SetAnimatorTrigger() {
        animator.SetTrigger("Appear");
    }

}
