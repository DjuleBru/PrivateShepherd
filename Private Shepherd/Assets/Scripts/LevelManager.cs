using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private Sheep[] sheepArray;
    private int initialSheepNumber;
    private int pennedSheepNumber = 0;

    [SerializeField] private float levelTimeLimit;
    private float levelTimer;

    public event EventHandler<OnScoreUpdateEventArgs> OnScoreUpdate;
    public event EventHandler<OnLevelFinishedEventArgs> OnLevelFinished;

    public class OnScoreUpdateEventArgs : EventArgs {
        public int initialSheepNumber;
        public int pennedSheepNumber;
    }

    public class OnLevelFinishedEventArgs : EventArgs {
        public int initialSheepNumber;
        public int pennedSheepNumber;
    }

    private void Awake() {
        Instance = this;
        sheepArray = FindObjectsOfType<Sheep>();
        initialSheepNumber = sheepArray.Length;
        levelTimer = levelTimeLimit;
    }

    private void Start() {
        foreach(Sheep sheep in sheepArray) {
            sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;
        }
    }

    private void Update() {
        levelTimer -= Time.deltaTime;

        if (levelTimer < 0) {
            OnLevelFinished?.Invoke(this, new OnLevelFinishedEventArgs {
                initialSheepNumber = initialSheepNumber,
                pennedSheepNumber = pennedSheepNumber
            });
        }
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, Sheep.OnSheepEnterScoreZoneEventArgs e) {
        pennedSheepNumber++;
        OnScoreUpdate?.Invoke(this, new OnScoreUpdateEventArgs {
            initialSheepNumber = initialSheepNumber,
            pennedSheepNumber = pennedSheepNumber
        });

        if (initialSheepNumber == pennedSheepNumber) {
            OnLevelFinished?.Invoke(this, new OnLevelFinishedEventArgs {
                initialSheepNumber = initialSheepNumber,
                pennedSheepNumber = pennedSheepNumber
            });
        }
    }

    public int GetPennedSheepNumber() {
        return pennedSheepNumber;
    }

    public int GetInitialSheepNumber() {
        return initialSheepNumber;
    }

    public float GetLevelTimeLimit() {
        return levelTimeLimit;
    }

    public float GetLevelTimer() {
        return levelTimer;
    }
}
