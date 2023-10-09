using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SetAIAnimatorParameters))]

public class Sheep : MonoBehaviour
{

    private Transform sheepParent;
    private Transform initialSheepParent;

    private List<Sheep> sheepsInObjectPool;
    [SerializeField] protected SheepMovement sheepMovement;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private SheepObjectPool subSheepObjectPool;

    [SerializeField] LayerMask sheepLayerMask;
    [SerializeField] Collider2D sheepCollider;

    public event EventHandler<OnSheepEnterScoreZoneEventArgs> OnSheepEnterScoreZone;
    private bool hasEnteredScoreZone;
    private bool hasBeenBit;

    [SerializeField] int sheepMinimumNumber = 3;
    [SerializeField] float herdRadius = 5f;
    private int herdNumber;

    [SerializeField] TextMeshPro herdNumberText;

    public class OnSheepEnterScoreZoneEventArgs : EventArgs {
        public Transform[] scoreZoneAggregatePointArray;
    }

    protected virtual void Awake() {
        sheepsInObjectPool = subSheepObjectPool.GetSheepsInObjectPoolList();
        initialSheepParent = this.transform.parent;

        subSheepObjectPool.OnSheepDied += subSheepObjectPool_OnSheepDied;
    }

    private void Start() {
    }


    private void Update() {
        CalculateHerdNumber();
    }

    private void CalculateHerdNumber() {
        herdNumber = Physics2D.OverlapCircleAll(transform.position, herdRadius, sheepLayerMask).Length;
        herdNumberText.text = herdNumber.ToString();
    }

    public int GetHerdNumber() {
        return herdNumber;
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
            int sheepNumberWithinTargetSheepRadius = potentialSheep.GetHerdNumber();

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0 & sheepNumberWithinTargetSheepRadius >= sheepMinimumNumber) {
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
                int sheepNumberWithinTargetSheepRadius = potentialSheep.GetHerdNumber();

                if (sheepNumberWithinTargetSheepRadius >= maxSheepNumberSurroundings) {
                    sheepWithMaxSheepSurrounding = potentialSheep.transform;
                    maxSheepNumberSurroundings = sheepNumberWithinTargetSheepRadius;
                }
            }
        }


        return sheepWithMaxSheepSurrounding;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.TryGetComponent<ScoreZone>(out ScoreZone scoreZone)) {

            if (!hasEnteredScoreZone) {

                // Select random aggregate point in score zone and pass it in event
                Transform[] aggregatePointArray = scoreZone.GetAggregatePointArray();
                OnSheepEnterScoreZone?.Invoke(this, new OnSheepEnterScoreZoneEventArgs {
                    scoreZoneAggregatePointArray = aggregatePointArray
                });
            }

            hasEnteredScoreZone = true;
            RemovePennedSheepFromObjectPool();
        }
    }

  

    public void SetSheepParent(Transform newParent) {
        sheepParent = newParent;
        this.transform.parent = newParent;
        this.transform.position = newParent.position;
        this.transform.rotation = newParent.rotation;
    }

    public void BiteSheep() {
        hasBeenBit = true;
        sheepMovement.enabled = false;
        sheepCollider.enabled = false;
    }

    public void DropSheep() {
        hasBeenBit = false;
        sheepMovement.enabled = true;
        sheepCollider.enabled = true;
        this.transform.parent = null;
        this.transform.rotation = Quaternion.identity;
    }

    public void EatSheep() {
        RemoveDeadSheepFromObjectPool();
        sheepMovement.UnSubscribeFromEvents();
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

}
