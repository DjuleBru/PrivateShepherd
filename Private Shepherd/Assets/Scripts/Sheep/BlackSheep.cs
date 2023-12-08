using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSheep : MonoBehaviour
{
    private List<Sheep> targetableSheepsinLevel;
    private Sheep[] initialSheepsInLevel;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;

    private bool fleetargetadded;
    void Start()
    {
        // Initialise sheep lists and arrays
        targetableSheepsinLevel = new List<Sheep>();

        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();
        foreach (Sheep sheep in initialSheepsInLevel) {
            targetableSheepsinLevel.Add(sheep);
        }

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;
        levelSheepObjectPool.OnSheepPenned += LevelSheepObjectPool_OnSheepPenned;
    }

    private void Update() {
        if(LevelIntroNPC.Instance.GetIntroFinished() & !fleetargetadded) {
            fleetargetadded = true;
            AddFleeTargetToOtherSheep();
        }
    }

    private void AddFleeTargetToOtherSheep() {
        foreach (Sheep s in targetableSheepsinLevel) {
            if(s.GetSheepType() != SheepType.blackSheep) {
                s.GetComponent<SheepMovement>().AddFleeTarget(GetComponent<FleeTarget>());
            }
        }
    }

    private void OnDestroy() {
        foreach(Sheep s in targetableSheepsinLevel) {
            s.GetComponent<SheepMovement>().RemoveFleeTarget(GetComponent<FleeTarget>());
        }
    }

    private void LevelSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }

    private void LevelSheepObjectPool_OnSheepPenned(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }
}
