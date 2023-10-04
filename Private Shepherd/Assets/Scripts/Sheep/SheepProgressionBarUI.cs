using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SheepProgressionBarUI : MonoBehaviour
{
    [SerializeField] private Sprite[] sheepProgressionBarArray;
    [SerializeField] private Image sheepProgressionBar;
    [SerializeField] private TextMeshProUGUI pennedAndTotalSheepNumber;

    private void Start() {
        LevelManager.Instance.OnScoreUpdate += LevelManager_OnSheepPenned;
        UpdateSheepTextUI(LevelManager.Instance.GetPennedSheepNumber(), LevelManager.Instance.GetInitialSheepNumber());
     }

    private void LevelManager_OnSheepPenned(object sender, LevelManager.OnScoreUpdateEventArgs e) {
        float pennedSheepPropotion = (float)e.pennedSheepNumber / (float)e.initialSheepNumber;
        UpdateSheepProgressionBarUI(pennedSheepPropotion);
        UpdateSheepTextUI(e.pennedSheepNumber, e.initialSheepNumber);
    }

    private void UpdateSheepProgressionBarUI(float pennedSheepProportion) {
        int progressionBarUINumber = (int)Math.Floor(pennedSheepProportion * 10);

        sheepProgressionBar.sprite = sheepProgressionBarArray[progressionBarUINumber];
    }

    private void UpdateSheepTextUI(int pennedSheepNumber, int initialSheepNumber) {

        string pennedSheepNumberString = pennedSheepNumber.ToString();
        string initialSheepNumberString = initialSheepNumber.ToString();

        if (pennedSheepNumber < 10) {
             pennedSheepNumberString = "0" + pennedSheepNumber;
        } 

        if (initialSheepNumber < 10) {
            initialSheepNumberString = "0" + initialSheepNumber;
        }

        pennedAndTotalSheepNumber.text = pennedSheepNumberString + "/" + initialSheepNumberString;
    }
}
