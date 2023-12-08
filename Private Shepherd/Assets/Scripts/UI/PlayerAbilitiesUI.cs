using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitiesUI : MonoBehaviour
{
    [SerializeField] GameObject runUI;
    [SerializeField] GameObject sneakUI;
    [SerializeField] GameObject barkUI;
    [SerializeField] GameObject growlUI;


    private bool growlUnlocked;
    private bool runUnlocked;
    private bool sneakUnlocked;
    private bool barkUnlocked;

    private void Start() {
        barkUnlocked = ES3.Load("barkUnlocked", false);
        growlUnlocked = ES3.Load("growlUnlocked", false);
        sneakUnlocked = ES3.Load("sneakUnlocked", false);
        runUnlocked = ES3.Load("runUnlocked", false);

       
        growlUI.SetActive(growlUnlocked);
        barkUI.SetActive(barkUnlocked);
        //sneakUI.SetActive(sneakUnlocked);
        runUI.SetActive(runUnlocked);
       


    }
}
