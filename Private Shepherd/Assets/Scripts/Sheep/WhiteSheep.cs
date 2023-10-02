using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteSheep : Sheep
{
    [SerializeField] private SheepMovement sheepMovement;
    [SerializeField] private GameObject[] fleeTargetArray;
    [SerializeField] private GameObject player;

    private void Start() {

        sheepMovement.AddFleeTarget(player);
        foreach (GameObject fleeTarget in fleeTargetArray) {
            sheepMovement.AddFleeTarget(fleeTarget);
        }
    }

}
