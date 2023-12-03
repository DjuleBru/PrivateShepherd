using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

public enum SheepType {
    whiteSheep,
    blackSheep,
    redSheep,
    greenSheep,
    blueSheep,
    goldSheep,
    goat,
};

[RequireComponent(typeof(SetAIAnimatorParameters))]
public class Sheep : MonoBehaviour
{

    private Transform sheepParent;
    private Transform initialSheepParent;

    private List<Sheep> sheepsInObjectPool;
    [SerializeField] protected SheepMovement sheepMovement;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private SheepObjectPool subSheepObjectPool;
    [SerializeField] private SheepType sheepType;

    [SerializeField] private OutOfScreenTargetIndicator outOfScreenTargetIndicator;

    [SerializeField] private FleeTarget[] fleeTargetArray;

    [SerializeField] Collider2D sheepCollider;

    public event EventHandler<OnSheepEnterScoreZoneEventArgs> OnSheepEnterScoreZone;
    private bool hasEnteredScoreZone;
    private bool hasBeenBit;

    [SerializeField] int sheepHerdMinimumNumber = 3;
    

    public class OnSheepEnterScoreZoneEventArgs : EventArgs {
        public Transform[] scoreZoneAggregatePointArray;
    }

    protected virtual void Awake() {
        sheepsInObjectPool = subSheepObjectPool.GetSheepsInObjectPoolList();
        initialSheepParent = this.transform.parent;

        subSheepObjectPool.OnSheepDied += subSheepObjectPool_OnSheepDied;
    }

    private void Start() {
        foreach (FleeTarget fleeTarget in fleeTargetArray) {
            sheepMovement.AddFleeTarget(fleeTarget);

            if(fleeTarget.TryGetComponent<WolfAI>(out WolfAI wolfAI)) {
                wolfAI.OnWolfDied += WolfAI_OnWolfDied;
            }
        }
    }

    private void WolfAI_OnWolfDied(object sender, WolfAI.OnWolfDiedEventArgs e) {
        sheepMovement.RemoveWolfFleeTarget(e.fleeTarget);
    }

    public Transform GetClosestSheepWithEnoughSheepSurrounding() {

        Transform closestSheepWithEnoughSheepSurrounding = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if (sheepsInObjectPool.Count == 1) {
            return null;
        }

        foreach (Sheep potentialSheep in sheepsInObjectPool) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            // Sheep surroundings
            int sheepNumberWithinTargetSheepRadius = potentialSheep.GetComponentInChildren<SheepHerd>().GetHerdNumber();

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0 & sheepNumberWithinTargetSheepRadius >= sheepHerdMinimumNumber) {
                closestDistanceSqr = dSqrToSheep;
                closestSheepWithEnoughSheepSurrounding = potentialSheep.transform;
            }
        }

        return closestSheepWithEnoughSheepSurrounding;
    }

    public Transform GetSheepWithMaxSheepSurrounding() {

        Transform sheepWithMaxSheepSurrounding = null;
        Vector3 currentPosition = transform.position;
        int maxSheepNumberSurroundings = 0;

        if (sheepsInObjectPool.Count == 1) {
            return null;
        }

        while (sheepWithMaxSheepSurrounding == null) {
            foreach (Sheep potentialSheep in sheepsInObjectPool) {

                // Distance to sheep
                Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;

                // Sheep surroundings
                int sheepNumberWithinTargetSheepRadius = potentialSheep.GetComponentInChildren<SheepHerd>().GetHerdNumber();

                if (sheepNumberWithinTargetSheepRadius >= maxSheepNumberSurroundings) {
                    sheepWithMaxSheepSurrounding = potentialSheep.transform;
                    maxSheepNumberSurroundings = sheepNumberWithinTargetSheepRadius;
                }
            }
        }


        return sheepWithMaxSheepSurrounding;
    }

    public void SheepEnteredScoreZone(ScoreZone scoreZone) {
        if (!hasEnteredScoreZone) {

            // Select random aggregate point in score zone and pass it in event
            Transform[] aggregatePointArray = scoreZone.GetAggregatePointArray();
            OnSheepEnterScoreZone?.Invoke(this, new OnSheepEnterScoreZoneEventArgs {
                scoreZoneAggregatePointArray = aggregatePointArray
            });
        }

        hasEnteredScoreZone = true;
        RemovePennedSheepFromObjectPool();
        outOfScreenTargetIndicator.DeActivateIndicator();
    }

    public void SetSheepParent(Transform newParent) {
        sheepParent = newParent;
        this.transform.parent = newParent;
        this.transform.position = newParent.position;
        this.transform.rotation = newParent.rotation;
    }

    public void BiteSheep() {
        hasBeenBit = true;
        sheepMovement.SetCanMove(false);
        sheepMovement.SetInjured(true);
        sheepCollider.enabled = false;
    }

    public void DropSheep() {
        hasBeenBit = false;
        sheepMovement.enabled = true;
        sheepCollider.enabled = true;
        this.transform.parent = null;
        this.transform.rotation = Quaternion.identity;

        sheepMovement.SetCanMove(true);
        sheepMovement.SetState(SheepMovement.State.ExtremeAggregate);
    }

    public void EatSheep() {
        RemoveDeadSheepFromObjectPool();
        sheepMovement.UnSubscribeFromEvents();

        foreach (FleeTarget fleeTarget in fleeTargetArray) {
            sheepMovement.AddFleeTarget(fleeTarget);
            if(fleeTarget != null) {
                if (fleeTarget.TryGetComponent<WolfAI>(out WolfAI wolfAI)) {
                    wolfAI.OnWolfDied -= WolfAI_OnWolfDied;
                }
            }
        }

        Destroy(gameObject);
    }
    public void RemoveDeadSheepFromObjectPool() {
        levelSheepObjectPool.RemoveDeadSheepFromObjectPool(this);
        subSheepObjectPool.RemoveDeadSheepFromObjectPool(this);
    }

    public void RemovePennedSheepFromObjectPool() {
        subSheepObjectPool.RemovePennedSheepFromObjectPool(this);
        levelSheepObjectPool.RemovePennedSheepFromObjectPool(this);
    }

    private void subSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
        sheepsInObjectPool = subSheepObjectPool.GetSheepsInObjectPoolList();
    }

    public bool GetHasBeenBit() {
        return hasBeenBit;
    }

    public SheepType GetSheepType() {
        return sheepType;
    }

}
