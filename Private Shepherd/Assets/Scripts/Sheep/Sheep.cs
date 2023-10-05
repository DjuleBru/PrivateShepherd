using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SetAIAnimatorParameters))]

public class Sheep : MonoBehaviour
{

    private Transform sheepParent;
    private Transform initialSheepParent;

    private Sheep[] sheepAggregateArray;
    [SerializeField] protected SheepMovement sheepMovement;
    [SerializeField] private SheepObjectPool subSheepObjectPool;

    [SerializeField] LayerMask sheepLayerMask;
    [SerializeField] Collider2D sheepCollider;

    public event EventHandler<OnSheepEnterScoreZoneEventArgs> OnSheepEnterScoreZone;
    private bool hasEnteredScoreZone;

    [SerializeField] int sheepMinimumNumber = 3;
    [SerializeField] float herdRadius = 5f;
    private int herdNumber;

    [SerializeField] TextMeshPro herdNumberText;

    public class OnSheepEnterScoreZoneEventArgs : EventArgs {
        public Transform[] scoreZoneAggregatePointArray;
    }

    protected virtual void Awake() {
        sheepAggregateArray = subSheepObjectPool.GetSheepArray();
        initialSheepParent = this.transform.parent;
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

        if (sheepAggregateArray.Length == 1) {
            return null;
        }

        foreach (Sheep potentialSheep in sheepAggregateArray) {

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
        }
    }

    public void SetSheepParent(Transform newParent) {
        sheepParent = newParent;
        this.transform.parent = newParent;
        this.transform.position = newParent.position;
        this.transform.rotation = newParent.rotation;
    }

    public void BiteSheep() {
        sheepMovement.enabled = false;
        sheepCollider.enabled = false;
    }

}
