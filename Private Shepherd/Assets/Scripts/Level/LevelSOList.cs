using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSOList : MonoBehaviour
{
    [SerializeField] List<LevelSO> levelSOList;

    public static LevelSOList Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public List<LevelSO> GetlevelSOList() {
        return levelSOList;
    }

}
