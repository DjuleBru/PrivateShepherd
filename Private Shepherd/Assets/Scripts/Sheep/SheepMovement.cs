using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sheep;

public class SheepMovement : AIMovement
{
    [SerializeField] private Sheep sheep;
    [SerializeField] private SheepSO sheepSO;
    private Transform closestSheepTransform;
    private Transform targetScoreAggregatePoint; 

    public event EventHandler OnSheepFlee;

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;
    #endregion

    #region SHEEP PARAMETERS
    private float triggerAggregateDistance;

    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;

    private float fleeSpeed;
    private float aggregateSpeed;
    private float roamSpeed;

    private bool penned;
    #endregion

    public enum State {
        Flee,
        Aggregate,
        Roam,
        InScoreZone,
    }

    private State state;

    private void Awake() {
        triggerAggregateDistance = sheepSO.triggerAggregateDistance;
        roamPauseMaxTime = sheepSO.roamPauseMaxTime;
        roamPauseMinTime = sheepSO.roamPauseMinTime;
        moveSpeed = sheepSO.moveSpeed;
        fleeSpeed = sheepSO.fleeSpeed;
        aggregateSpeed = sheepSO.aggregateSpeed;
        roamSpeed = sheepSO.roamSpeed;

        state = State.Aggregate;
    }

    protected override void Start() {
        seeker = GetComponent<Seeker>();
        PlayerBark.Instance.OnPlayerBark += PlayerBark_OnPlayerBark;
        sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;

        //Initialise path
        CalculatePath(transform.position);
        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
    }

    private void Update() {

        if (path == null) {
            return;
        }

        closestFleeTarget = FindClosestFleeTarget();
        UpdateClosestFleeTargetParameters();

        switch (state) {
            case State.Flee:
                moveSpeed = fleeSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;

                } else {
                    // Target is within flee stop radius
                    CalculateFleePath(closestFleeTarget.transform.position);
                }

                FollowPath(path);

            break;
            case State.Aggregate:
                moveSpeed = aggregateSpeed;
                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    OnSheepFlee?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (Vector3.Distance(closestSheepTransform.position, transform.position) < triggerAggregateDistance) {
                    reachedEndOfPath = true;
                    state = State.Roam;
                    roamPauseTimer = 0;
                }

                CalculatePath(closestSheepTransform.position);

                FollowPath(path);

            break;
            case State.Roam:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    OnSheepFlee?.Invoke(this, EventArgs.Empty);
                    return;
                }

                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();
                if (closestSheepTransform != null) {
                    // There is another sheep to aggregate
                    if (Vector3.Distance(closestSheepTransform.position, transform.position) > triggerAggregateDistance) {
                        state = State.Aggregate;
                    }
                }
                
                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint(transform.position);
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                FollowPath(path);
                break;
            case State.InScoreZone:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;

                float pennedDistanceTreshold = 1f;
                if (Vector3.Distance(targetScoreAggregatePoint.position, transform.position) < pennedDistanceTreshold) {
                    // Check if sheep is close enough to aggregatePoint
                    penned = true;
                }

                if(!penned) {
                    CalculatePath(targetScoreAggregatePoint.position);
                } else {
                    if (roamPauseTimer <= 0) {
                        Vector3 roamDestination = PickRandomPoint(targetScoreAggregatePoint.position);
                        CalculatePath(roamDestination);
                        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                    }
                }

                FollowPath(path);
                break;
        }
    }

    private FleeTarget FindClosestFleeTarget() {

        float fleeTargetDistance;

        foreach (FleeTarget fleeTarget in fleeTargetList) {
            fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);

            if (fleeTargetDistance < closestFleeTargetDistance) {
                // The flee target is the closest one

                closestFleeTargetDistance = fleeTargetDistance;
                closestFleeTarget = fleeTarget;
            }
        }

        return closestFleeTarget;
    }

    private void UpdateClosestFleeTargetParameters() {
        closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, transform.position);
        closestFleeTargetTriggerFleeDistance = closestFleeTarget.GetFleeTargetTriggerDistance();
        closestFleeTargetStopDistance = closestFleeTarget.GetFleeTargetStopDistance();
    }

    private void PlayerBark_OnPlayerBark(object sender, System.EventArgs e) {
        StartCoroutine(SetFleeSpeed(sheepSO.barkFleeSpeed));
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, OnSheepEnterScoreZoneEventArgs e) {
        state = State.InScoreZone;
        Transform[] scoreZoneAggregatePointArray = e.scoreZoneAggregatePointArray;

        // pick a random aggregate point in scorezone
        targetScoreAggregatePoint = scoreZoneAggregatePointArray[UnityEngine.Random.Range(0, scoreZoneAggregatePointArray.Length)];
    }

    public void AddFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Add(fleeTarget);
    }

    public IEnumerator SetFleeSpeed(float newFleeSpeed) {
        fleeSpeed = newFleeSpeed;

        yield return new WaitForSeconds(1f);

        fleeSpeed = sheepSO.fleeSpeed;
    }

}
