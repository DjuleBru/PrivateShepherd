using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakUI : MonoBehaviour
{
    private bool sneakUnlocked;

    private void Start() {

        sneakUnlocked = PlayerGrowl.Instance.GetGrowlUnlocked();
        if (!sneakUnlocked) {
            gameObject.SetActive(false);
        }

    }
}
