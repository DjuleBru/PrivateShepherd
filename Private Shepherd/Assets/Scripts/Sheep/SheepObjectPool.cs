using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepObjectPool : MonoBehaviour
{
    // Put this script first in script execution order (edit > project settings)

    private Sheep[] initialSheepsInLevel;
    private List<Sheep> sheepsAliveAndUnpennedInObjectPool;
    private List<Sheep> sheepsAliveInObjectPool;

    public event EventHandler OnSheepDied;
    public event EventHandler OnSheepPenned;

    private void Awake() {
        initialSheepsInLevel = GetComponentsInChildren<Sheep>();

        //Initialise sheep List
        sheepsAliveInObjectPool = new List<Sheep>();
        sheepsAliveAndUnpennedInObjectPool = new List<Sheep>();

        foreach (Sheep sheep in initialSheepsInLevel) {
            sheepsAliveInObjectPool.Add(sheep);
            sheepsAliveAndUnpennedInObjectPool.Add(sheep);
        }
    }

    public Sheep[] GetSheepArray() { 
        return initialSheepsInLevel; 
    }

    public List<Sheep> GetSheepsInObjectPoolList() {
        return sheepsAliveInObjectPool;
    }

    public List<Sheep> GetSheepsInUnPennedObjectPoolList() {
        return sheepsAliveAndUnpennedInObjectPool;
    }

    public void RemoveDeadSheepFromObjectPool(Sheep sheep) {
        sheepsAliveInObjectPool.Remove(sheep);
        sheepsAliveAndUnpennedInObjectPool.Remove(sheep);
        OnSheepDied?.Invoke(this, EventArgs.Empty);
    }

    public void RemovePennedSheepFromObjectPool(Sheep sheep) {
        sheepsAliveAndUnpennedInObjectPool.Remove(sheep);
        OnSheepPenned?.Invoke(this, EventArgs.Empty);
    }

    public void DeactivateSheepTargetIndicators() {
        foreach(Sheep sheep in sheepsAliveInObjectPool) {
            sheep.gameObject.GetComponent<OutOfScreenSheepIndicator>().DeActivateIndicator();
        }
    }

    public void ActivateSheepTargetIndicators() {
        foreach (Sheep sheep in sheepsAliveInObjectPool) {
            sheep.gameObject.GetComponent<OutOfScreenSheepIndicator>().ActivateIndicator();
        }
    }
}
