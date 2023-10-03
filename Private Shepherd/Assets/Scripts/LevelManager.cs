using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private Sheep[] sheepArray;
    private int sheepNumber;
    private int levelScore = 0;

    private void Start() {
        sheepArray = FindObjectsOfType<Sheep>();
        sheepNumber = sheepArray.Length;

        foreach(Sheep sheep in sheepArray) {
            sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;
        }
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, Sheep.OnSheepEnterScoreZoneEventArgs e) {
        levelScore++;
        Debug.Log(levelScore.ToString());

    }
}
