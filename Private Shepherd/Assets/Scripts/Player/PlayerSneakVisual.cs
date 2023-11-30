using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSneakVisual : MonoBehaviour
{

    private Animator animator;
    private void Start() {
        PlayerSneak.Instance.OnPlayerSneakReleased += PlayerSneak_OnPlayerSneakReleased;
        PlayerSneak.Instance.OnPlayerSneakStarted += PlayerSneak_OnPlayerSneakStarted;

        animator = GetComponent<Animator>();
    }

    private void PlayerSneak_OnPlayerSneakStarted(object sender, System.EventArgs e) {
        animator.SetBool("isSneaking", true);
    }

    private void PlayerSneak_OnPlayerSneakReleased(object sender, System.EventArgs e) {
        animator.SetBool("isSneaking", false);
    }
}
