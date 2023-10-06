using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SheepProgressionBarUI : MonoBehaviour
{
    [SerializeField] private Sprite[] sheepPennedProgressionBarArray;
    [SerializeField] private Image sheepPennedProgressionBar;

    [SerializeField] private Sprite[] sheepDeadProgressionBarArray;
    [SerializeField] private Image sheepDeadProgressionBar;

    [SerializeField] private TextMeshProUGUI pennedAndTotalSheepNumber;

    private int initialSheepNumber;

    private void Start() {
        LevelManager.Instance.OnScoreUpdate += LevelManager_OnScoreUpdate;
        initialSheepNumber = LevelManager.Instance.GetInitialSheepNumber();
        UpdateSheepTextUI(LevelManager.Instance.GetPennedSheepNumber(), LevelManager.Instance.GetInitialSheepNumber());
     }

    private void LevelManager_OnScoreUpdate(object sender, LevelManager.OnScoreUpdateEventArgs e) {
        int deadSheepNumber = initialSheepNumber - e.realTimeSheepNumber;
        float deadSheepProportion = (float)deadSheepNumber / (float)initialSheepNumber;
        float pennedSheepPropotion = (float)e.pennedSheepNumber / (float)initialSheepNumber;

        UpdateSheepProgressionBarUI(pennedSheepPropotion, deadSheepProportion);
        UpdateSheepTextUI(e.pennedSheepNumber, e.realTimeSheepNumber);
    }

    private void UpdateSheepProgressionBarUI(float pennedSheepProportion, float deadSheepProportion) {
        int progressionBarUIPennedNumber = (int)Math.Floor(pennedSheepProportion * 10);
        int progressionBarUIDeadNumber = (int)Math.Ceiling(deadSheepProportion * 10);

        sheepPennedProgressionBar.sprite = sheepPennedProgressionBarArray[progressionBarUIPennedNumber];
        sheepDeadProgressionBar.sprite = sheepDeadProgressionBarArray[progressionBarUIDeadNumber];
    }

    private void UpdateSheepTextUI(int pennedSheepNumber, int realTimeSheepNumber) {

        string pennedSheepNumberString = pennedSheepNumber.ToString();
        string realTimeSheepNumberString = realTimeSheepNumber.ToString();

        if (pennedSheepNumber < 10) {
             pennedSheepNumberString = "0" + pennedSheepNumber;
        } 

        if (realTimeSheepNumber < 10) {
            realTimeSheepNumberString = "0" + realTimeSheepNumber;
        }

        pennedAndTotalSheepNumber.text = pennedSheepNumberString + "/" + realTimeSheepNumberString;
    }
}
