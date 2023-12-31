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

    [SerializeField] private float ramDistance;
    [SerializeField] private float hitDistance;
    [SerializeField] private float ramCooldown;
    [SerializeField] private float ramForce;
    private bool ramming;
    private bool ramWarning;
    private float ramTimer;

    public event EventHandler OnGoatRamWarning;
    public event EventHandler OnGoatRam;
    public event EventHandler OnGoatRamEnd;
    public static event EventHandler OnAnyGoatRamWarning;
    public static event EventHandler OnAnyGoatRam;

    private Transform closestSheepTransform;
    private Transform targetScoreAggregatePoint; 

    public event EventHandler OnSheepFlee;
    public event EventHandler OnSheepInjured;
    public static event EventHandler OnAnySheepFlee;
    public static event EventHandler OnAnySheepFleeAggregate;

    private bool canMove;
    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    float closestFleeTargetDistance = float.MaxValue;
    float closestFleeTargetTriggerFleeDistance;
    float closestFleeTargetStopDistance;
    float closestFleeTargetStopDistanceModifier;

    private Sheep sheepInHerdFleeing;
    private bool wolfDied;
    #endregion

    #region SHEEP PARAMETERS
    private float triggerAggregateDistance;

    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;

    private float aggregateSpeed;
    private float roamSpeed;
    private float originalMoveSpeed;
    private float injuredSpeedFactor;
    private float rbMass;

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
        Ram,
    }

    private State state;

    private void Awake() {
        RetreiveSheepParameters();
        nextWaypointDistance = 1.5f;

        state = State.Roam;
    }

    protected override void Start() {

        seeker = GetComponent<Seeker>();
        //Initialise path
        CalculatePath(transform.position);
        RandomizeFleeTargetModifiers();

        sheep.OnSheepEnterScoreZone += Sheep_OnSheepEnterScoreZone;
        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
        canMove = true;
    }

    private void Update() {

        if (path == null | !canMove) {
            return;
        }

        if (cutSceneInProgress) {
            state = State.Roam;
        }

        if (!wolfDied) {
            closestFleeTarget = FindClosestFleeTarget();
        }

        if (closestFleeTarget == null) {
            return;
        }

        UpdateClosestFleeTargetParameters();
        CheckRamConditions();

        closestSheepTransform = sheep.GetClosestSheepWithEnoughSheepSurrounding();
        if (closestSheepTransform == null) {
            closestSheepTransform = sheep.GetSheepWithMaxSheepSurrounding();
        }

        sheepInHerdFleeing = sheepHerd.GetSheepInHerdFleeing();

        FollowPath(path);
        switch (state) {
            case State.Flee:
                float fleeSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();
                moveSpeed = fleeSpeed * 1.2f;
                rb.mass = rbMass * 4f;

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;

                }
                else {
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
                moveSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();
                rb.mass = rbMass;

                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Aggregate;
                    return;
                }

                // Follow closest sheep fleeing
                if (sheepInHerdFleeing != null) {
                    FollowClosestSheepFleeing();
                }
                else {
                    // Closest target is in flee stop radius
                    state = State.Flee;
                }

                break;
            case State.Aggregate:
                fleeLeader = false;
                moveSpeed = aggregateSpeed;
                rb.mass = rbMass;

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                if (closestFleeTarget != Player.Instance.GetComponent<FleeTarget>()) {
                    CheckFleeConditions();
                }

                CheckRoamConditions();
                CalculatePath(closestSheepTransform.position);

                break;
            case State.Roam:
                fleeLeader = false;
                moveSpeed = roamSpeed;
                rb.mass = rbMass;
                roamPauseTimer -= Time.deltaTime * timeScale;

                CheckFleeConditions();

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
                moveSpeed = originalMoveSpeed * closestFleeTarget.GetFleeTargetSpeedMultiplier();
                rb.mass = rbMass;

                if (closestSheepTransform == null) {
                    // There is no other sheep to aggregate to
                    state = State.Roam;
                    return;
                }

                CheckRoamConditions();

                CalculatePath(closestSheepTransform.position);
                break;

            case State.InScoreZone:
                rb.mass = rbMass;
                roamPauseTimer -= Time.deltaTime * timeScale;

                float pennedDistanceTreshold = 1f;
                if (Vector3.Distance(targetScoreAggregatePoint.position, transform.position) < pennedDistanceTreshold) {
                    // Check if sheep is close enough to aggregatePoint
                    penned = true;
                }

                if (!penned) {
                    moveSpeed = aggregateSpeed;
                    CalculatePath(targetScoreAggregatePoint.position);
                }
                else {
                    if (roamPauseTimer <= 0) {
                        moveSpeed = roamSpeed;
                        Vector3 roamDestination = PickRandomPoint(targetScoreAggregatePoint.position);
                        CalculatePath(roamDestination);
                        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                    }
                }

                break;
            case State.Ram:
                if (Vector3.Distance(Player.Instance.transform.position, transform.position) > hitDistance) {
                    moveSpeed = aggregateSpeed * 2;
                    CalculatePath(Player.Instance.transform.position);
                }
                else {
                    Vector3 forceDirection = (transform.position - Player.Instance.transform.position).normalized;
                    Vector3 force = new Vector3(forceDirection.x, forceDirection.y, 0) * ramForce;
                    OnGoatRamEnd?.Invoke(this, EventArgs.Empty);
                    Player.Instance.GetComponent<PlayerMovement>().Rammed(force);
                    ramming = false;
                    ramTimer = ramCooldown;
                    state = State.Roam;
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

    private void FollowClosestSheepFleeing() {
        // Pick a point in the direction flee leader is going
        float forwardDirectionMultiplier = 6f;
        Vector3 destination = sheepInHerdFleeing.transform.position + sheepInHerdFleeing.GetComponentInParent<SheepMovement>().GetMoveDirNormalized() * forwardDirectionMultiplier;

        // Pick a random point around direction flee leader is going
        float randomizer = 4f;
        destination = destination + new Vector3(UnityEngine.Random.Range(-randomizer, randomizer), UnityEngine.Random.Range(-randomizer, randomizer), 0);
        CalculatePath(destination);
    }

    private void CheckRoamConditions() {
        if (Vector3.Distance(closestSheepTransform.position, transform.position) < triggerAggregateDistance) {
            reachedEndOfPath = true;
            state = State.Roam;
            roamPauseTimer = 0;
        }
    }

    private void CheckFleeConditions() {
        if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
            // Target is within flee trigger radius
            if (sheepInHerdFleeing == null) {
                // There is no other sheep fleeing within flee trigger radius
                fleeLeader = true;
                state = State.Flee;
                OnSheepFlee?.Invoke(this, EventArgs.Empty);
                OnAnySheepFlee?.Invoke(this, EventArgs.Empty);
            }
            else {
                state = State.FleeAggregate;
                OnAnySheepFleeAggregate?.Invoke(this, EventArgs.Empty);
                return;
            }
            return;
        }

        if (closestFleeTargetDistance <= closestFleeTargetStopDistance) {
            if (sheepInHerdFleeing != null) {
                // There is another sheep fleeing within flee trigger radius
                state = State.FleeAggregate;
                OnAnySheepFleeAggregate?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    private void RetreiveSheepParameters() {
        triggerAggregateDistance = sheepSO.triggerAggregateDistance;
        roamPauseMaxTime = sheepSO.roamPauseMaxTime;
        roamPauseMinTime = sheepSO.roamPauseMinTime;
        originalMoveSpeed = sheepSO.moveSpeed;
        aggregateSpeed = sheepSO.aggregateSpeed;
        roamSpeed = sheepSO.roamSpeed;
        injuredSpeedFactor = sheepSO.injuredSpeedFactor;

        rbMass = rb.mass;
    }

    private void RandomizeFleeTargetModifiers() {
        float playerFleeTargetStopDistance = Player.Instance.GetComponent<FleeTarget>().GetFleeTargetStopDistance();
        closestFleeTargetStopDistanceModifier = UnityEngine.Random.Range(-playerFleeTargetStopDistance / 5, playerFleeTargetStopDistance / 5);
    }

    private void UpdateClosestFleeTargetParameters() {
        if (closestFleeTarget == null) {
            return;
        }
        closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, transform.position);
        closestFleeTargetTriggerFleeDistance = closestFleeTarget.GetFleeTargetTriggerDistance();
        closestFleeTargetStopDistance = closestFleeTarget.GetFleeTargetStopDistance() + closestFleeTargetStopDistanceModifier;

        if (closestFleeTargetStopDistance <= closestFleeTargetTriggerFleeDistance) {
            // There was a problem with the randomizer
            closestFleeTargetStopDistance = closestFleeTargetTriggerFleeDistance;
        }
    }

    private void Sheep_OnSheepEnterScoreZone(object sender, OnSheepEnterScoreZoneEventArgs e) {
        state = State.InScoreZone;

        Transform[] scoreZoneAggregatePointArray = e.scoreZoneAggregatePointArray;

        // pick a random aggregate point in scorezone
        targetScoreAggregatePoint = scoreZoneAggregatePointArray[UnityEngine.Random.Range(0, scoreZoneAggregatePointArray.Length)];
    }

    private void CheckRamConditions() {
        if (sheep.GetSheepType() == SheepType.goat) {
            ramTimer -= Time.deltaTime * timeScale;
            if (Vector3.Distance(Player.Instance.transform.position, transform.position) < ramDistance & ramTimer < 0 & !ramWarning) {
                ramWarning = true;
                OnGoatRamWarning?.Invoke(this, EventArgs.Empty);

                Invoke("Ram", 1f);
            }
        }
    }

    private void Ram() {
        OnGoatRam?.Invoke(this, EventArgs.Empty);
        ramming = true;
        state = State.Ram;
        ramWarning = false;
    }

    public void AddFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Add(fleeTarget);
    }
    public void RemoveFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Remove(fleeTarget);
        closestFleeTarget = FindClosestFleeTarget();
    }

    public void RemoveWolfFleeTarget(FleeTarget fleeTarget) {
        wolfDied = true;
        fleeTargetList.Remove(fleeTarget);
        closestFleeTarget = FindClosestFleeTarget();
        wolfDied = false;
    }

    public void UnSubscribeFromEvents() {
        sheep.OnSheepEnterScoreZone -= Sheep_OnSheepEnterScoreZone;
    }

    public State GetState() {
        return state;
    }

    public void SetState(State state) {
        this.state = state;
    }



    public void SetCanMove(bool canMove) {
        velocity = Vector3.zero;

        this.canMove = canMove;
    }

    public void SetInjured(bool injured) {
        if (!this.injured) {
            OnSheepInjured?.Invoke(this, EventArgs.Empty);
            originalMoveSpeed = originalMoveSpeed * injuredSpeedFactor;
        }
        this.injured = injured;
    }


}