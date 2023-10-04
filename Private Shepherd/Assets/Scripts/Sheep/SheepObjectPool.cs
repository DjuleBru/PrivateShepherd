using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepObjectPool : MonoBehaviour
{
    // Put this script first in script execution order (edit > project settings)

    private Sheep[] initialSheepArray;
    private List<Sheep> sheepsInObjectPoolList;

    private void Awake() {
        initialSheepArray = GetComponentsInChildren<Sheep>();

        //Initialise sheep List
        sheepsInObjectPoolList = new List<Sheep>();

        foreach (Sheep sheep in initialSheepArray) {
            sheepsInObjectPoolList.Add(sheep);
        }

    }

    public Sheep[] GetSheepArray() { 
        return initialSheepArray; 
    }

    public List<Sheep> GetSheepsInObjectPoolList() {
        return sheepsInObjectPoolList;
    }
}
