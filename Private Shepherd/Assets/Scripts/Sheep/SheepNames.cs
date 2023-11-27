using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SheepNames : MonoBehaviour
{
    [SerializeField] private List<string> sheepNames = new List<string>();
    private List<string> levelSheepNames = new List<string>();

    public static SheepNames Instance { get; private set; }

    private void Awake() {
        Instance = this;

        foreach(string s in sheepNames) {
            levelSheepNames.Add(s);
        }
    }

    public List<string> GetLevelSheepNamesList() {
        return levelSheepNames;
    }

    public void RemoveSheepNameFromList(string name) {
        levelSheepNames.Remove(name);
    }

}
