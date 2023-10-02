using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepObjectPool : MonoBehaviour
{
    // Put this script first in script execution order (edit > project settings)
    public static SheepObjectPool Instance { get; private set; }

    private Sheep[] sheepArray;

    private void Awake() {
        Instance = this;
        sheepArray = GetComponentsInChildren<Sheep>();
    }

    public Sheep[] GetSheepArray() { 
        return sheepArray; 
    }
}
