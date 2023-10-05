using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepObjectPool : MonoBehaviour
{
    // Put this script first in script execution order (edit > project settings)

    private Sheep[] initialSheepsInLevel;
    private List<Sheep> sheepsInObjectPool;

    public event EventHandler OnSheepRemoved;

    private void Awake() {
        initialSheepsInLevel = GetComponentsInChildren<Sheep>();

        //Initialise sheep List
        sheepsInObjectPool = new List<Sheep>();

        foreach (Sheep sheep in initialSheepsInLevel) {
            sheepsInObjectPool.Add(sheep);
        }
    }

    public Sheep[] GetSheepArray() { 
        return initialSheepsInLevel; 
    }

    public List<Sheep> GetSheepsInObjectPoolList() {
        return sheepsInObjectPool;
    }

    public void RemoveSheepFromObjectPool(Sheep sheep) {
        sheepsInObjectPool.Remove(sheep);
        OnSheepRemoved?.Invoke(this, EventArgs.Empty);
    }
}
