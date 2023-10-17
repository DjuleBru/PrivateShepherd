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
    [SerializeField] private SheepHerd sheepHerd;
    private Transform closestSheepTransform;
    private Transform targetScoreAggregatePoint; 

    public event EventHandler OnSheepFlee;
    public event EventHandler OnSheepInjured;

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;

    private Sheep sheepInHerdFleeing;
    #endregion

    #region SHEEP PARAMETERS
    private float triggerAggregateDistance;

    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;

    private float fleeSpeed;
    private float aggregateSpeed;
    private float roamSpeed;
    private float originalMoveSpeed;
    private float injuredSpeedFactor;

    private bool penned;
    private bool injured;
    private bool fleeLeader;
    #endregion

    public enum State {
        Flee,
        FleeAggregate,
        Aggregate,
        Roam,
        ExtremeAggregate,
        InScoreZone,
    }

    private State state;

    private void Awake() {

        RetreiveSheepParameters();

        nextWaypointDistance = 1.5f;

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

        sheepInHerdFleeing = sheepHerd.GetSheepInHerdFleeing();

        FollowPath(path);

        switch (state) {
            case State.Flee:
                moveSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;

                } else {
                    // Target is within flee radius
                    // Check if there is another sheep fleeing in herd
                    if (sheepInHerdFleeing == null) {
                        fleeLeader = true;
                    }
                    if (!fleeLeader) {
                        state = State.FleeAggregate;
                        return;
                    }

                    CalculateFleePath(closestFleeTarget.transform.position);
                }

            break;
            case State.FleeAggregate:
                float fleeSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();
                moveSpeed = fleeSpeed * 1.1f;

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;

                }

                // Follow closest sheep fleeing
                if (sheepInHerdFleeing != null) {

                    // Pick a point in the direction flee leader is going
                    float forwardDirectionMultiplier = 6f;
                    Vector3 destination = sheepInHerdFleeing.transform.position + sheepInHerdFleeing.GetComponentInParent<SheepMovement>().GetMoveDirNormalized() * forwardDirectionMultiplier;

                    // Pick a random point around direction flee leader is going
                    float randomizer = 4f;
                    destination = destination + new Vector3(UnityEngine.Random.Range(-randomizer, randomizer), UnityEngine.Random.Range(-randomizer, randomizer), 0);
                    CalculatePath(destination);
                } else {
                    state = State.Flee;
                }

                break;
            case State.Aggregate:
                fleeLeader = false;
                moveSpeed = aggregateSpeed;
                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();
                if (closestSheepTransform == null) {
                    closestSheepTransform = sheep.GetSheepWithMaxSheepSurrounding();
                }

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

            break;
            case State.Roam:
                fleeLeader = false;
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

                break;

            case State.ExtremeAggregate:
                fleeLeader = false;
                moveSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier(); ;
                closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();
                if (closestSheepTransform == null) {
                    closestSheepTransform = sheep.GetSheepWithMaxSheepSurrounding();
                }

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                if (Vector3.Distance(closestSheepTransform.position, transform.position) < triggerAggregateDistance) {
                    reachedEndOfPath = true;
                    state = State.Roam;
                    roamPauseTimer = 0;
                }

                CalculatePath(closestSheepTransform.position);
                break;

            case State.InScoreZone:
                roamPauseTimer -= Time.deltaTime;

                float pennedDistanceTreshold = 1f;
                if (Vector3.Distance(targetScoreAggregatePoint.position, transform.position) < pennedDistanceTreshold) {
                    // Check if sheep is close enough to aggregatePoint
                    penned = true;
                }

                if(!penned) {
                    moveSpeed = aggregateSpeed;
                    CalculatePath(targetScoreAggregatePoint.position);
                } else {
                    if (roamPauseTimer <= 0) {
                        moveSpeed = roamSpeed;
                        Vector3 roamDestination = PickRandomPoint(targetScoreAggregatePoint.position);
                        CalculatePath(roamDestination);
                        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                    }
                }

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

    private void RetreiveSheepParameters() {
        triggerAggregateDistance = sheepSO.triggerAggregateDistance;
        roamPauseMaxTime = sheepSO.roamPauseMaxTime;
        roamPauseMinTime = sheepSO.roamPauseMinTime;
        originalMoveSpeed = sheepSO.moveSpeed;
        aggregateSpeed = sheepSO.aggregateSpeed;
        roamSpeed = sheepSO.roamSpeed;
        injuredSpeedFactor = sheepSO.injuredSpeedFactor;
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

    public void UnSubscribeFromEvents() {
        PlayerBark.Instance.OnPlayerBark -= PlayerBark_OnPlayerBark;
        sheep.OnSheepEnterScoreZone -= Sheep_OnSheepEnterScoreZone;
    }

    public State GetState() { 
        return state; 
    }

    public void SetState(State state) {
        this.state = state;
    }

    public void SetInjured(bool injured) {
        if (!this.injured) {
            OnSheepInjured?.Invoke(this, EventArgs.Empty);
            originalMoveSpeed = originalMoveSpeed * injuredSpeedFactor;
        }
        this.injured = injured;
    }


}
