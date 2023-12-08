using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunUI : MonoBehaviour
{
    private bool runUnlocked;
    [SerializeField] GameObject abilitiesPanelUI;
    void Start()
    {
        runUnlocked = PlayerRun.Instance.GetRunUnlocked();
        if (!runUnlocked) {
            gameObject.SetActive(false);
        } else {
            abilitiesPanelUI.SetActive(true);
        }
    }

}
