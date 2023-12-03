using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpyWolf : MonoBehaviour
{

    [SerializeField] GameObject wolf;
    [SerializeField] GameObject sheep;
    [SerializeField] OutOfScreenTargetIndicator sheepIndicator;
    [SerializeField] OutOfScreenTargetIndicator wolfIndicator;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private float distanceToActivateWolf;

    private Sheep closestSheep;
    private List<Sheep> targetableSheepsinLevel;
    private Sheep[] initialSheepsInLevel;

    [SerializeField] private float activationTime;
    private float activationTimer;

    private bool activated;
    private bool busted;
    private float bustedTimer;
    [SerializeField] private float bustedTime;

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;
    float closestFleeTargetStopDistanceModifier;
    #endregion

    private State state;
    public enum State {
        Sheep,
        Wolf,
    }
    void Start()
    {
        fleeTargetList.Add(Player.Instance.GetComponent<FleeTarget>()); 
        bustedTimer = bustedTime;

        sheepIndicator.DeActivateIndicator();
        wolfIndicator.DeActivateIndicator();    

        activationTimer = activationTime;

        wolf.SetActive(false);
        sheep.SetActive(true);

        // Initialise sheep lists and arrays
        targetableSheepsinLevel = new List<Sheep>(); 

        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();
        foreach (Sheep sheep in initialSheepsInLevel) {
            targetableSheepsinLevel.Add(sheep);
        }

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;
        levelSheepObjectPool.OnSheepPenned += LevelSheepObjectPool_OnSheepPenned;

        state = State.Sheep;
    }

    
    void Update()
    {

        if(!activated) {
            sheep.transform.position = transform.position;
            wolf.transform.position = transform.position;
            return;
        }

        closestSheep = FindClosestSheep();
        closestFleeTarget = FindClosestFleeTarget();
        UpdateClosestFleeTargetParameters();


        switch (state) {
            case State.Sheep:

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance & sheep.GetComponentInChildren<SheepHerd>().GetHerd().Count == 0) {
                    ActivateWolf();
                    wolf.GetComponent<WolfAI>().SetState(WolfAI.State.Flee);
                    return;
                }

                float distanceToClosestSheep = Vector3.Distance(closestSheep.transform.position, sheep.transform.position);

                if (distanceToClosestSheep < distanceToActivateWolf) {
                    activationTimer -= Time.deltaTime;
                    if (activationTimer < 0) {
                        ActivateWolf();
                    }
                    return;
                }

                break;
            case State.Wolf:

                bustedTimer -= Time.deltaTime;
                if (bustedTimer < 0) {
                    ActivateSheep();
                    bustedTimer = bustedTime;
                    busted = false;
                }

                break;
        }
    }

    private void ActivateSheep() {
        sheep.transform.position = wolf.transform.position;
        state = State.Sheep;
        wolf.SetActive(false);
        sheep.SetActive(true);

        foreach (Sheep sheep in targetableSheepsinLevel) {
            sheep.GetComponent<SheepMovement>().RemoveFleeTarget(wolf.GetComponent<FleeTarget>());
        }
    }

    private void ActivateWolf() {

        wolf.GetComponentInChildren<Animator>().ResetTrigger("Attack");
        activationTimer = activationTime;
        wolf.SetActive(true);
        sheep.SetActive(false);

        foreach (Sheep sheep in targetableSheepsinLevel) {
            sheep.GetComponent<SheepMovement>().AddFleeTarget(wolf.GetComponent<FleeTarget>());
        }
        wolf.transform.position = sheep.transform.position;
        state = State.Wolf;
    }

    private Sheep FindClosestSheep() {

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Sheep potentialSheep in targetableSheepsinLevel) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - sheep.transform.position;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0) {
                closestDistanceSqr = dSqrToSheep;
                closestSheep = potentialSheep;
            }
        }

        return closestSheep;
    }

    private FleeTarget FindClosestFleeTarget() {

        float fleeTargetDistance;

        foreach (FleeTarget fleeTarget in fleeTargetList) {
            fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, sheep.transform.position);

            if (fleeTargetDistance < closestFleeTargetDistance) {
                // The flee target is the closest one

                closestFleeTargetDistance = fleeTargetDistance;
                closestFleeTarget = fleeTarget;
            }
        }

        return closestFleeTarget;
    }
    private void UpdateClosestFleeTargetParameters() {
        if (closestFleeTarget == null) {
            return;
        }
        closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, sheep.transform.position);
        closestFleeTargetTriggerFleeDistance = closestFleeTarget.GetFleeTargetTriggerDistance();
        closestFleeTargetStopDistance = closestFleeTarget.GetFleeTargetStopDistance() + closestFleeTargetStopDistanceModifier;

        if (closestFleeTargetStopDistance <= closestFleeTargetTriggerFleeDistance) {
            // There was a problem with the randomizer
            closestFleeTargetStopDistance = closestFleeTargetTriggerFleeDistance;
        }
    }

    private void LevelSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }

    private void LevelSheepObjectPool_OnSheepPenned(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }

    public void Activate() {
        activated = true;
    }
}
