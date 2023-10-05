using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class WolfMovement : AIMovement {

    [SerializeField] private WolfSO wolfSO;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private Transform sheepHoldPoint;

    private bool inverseDirection;

    private State state;
    public enum State {
        Flee,
        Roam,
        Agressive,
        AgressiveFlee,
        Attack,
        EatSheepFlee,
    }

    #region TARGET SHEEP PARAMETERS
    [SerializeField] LayerMask sheepLayerMask;
    private List<Sheep> sheepsinLevel;
    private Sheep[] initialSheepsInLevel;
    private Sheep closestSheep;

    private Sheep closestAttackTargetSheep;
    private Sheep attackTargetSheep;

    private Sheep agressiveTargetSheep;
    private float agressiveTargetSheepTimer;
    private Vector3 vectorToClosestSheep;
    private float distanceToClosestSheep;

    float fleeClosestSheepDestinationMultiplier = 3f;
    float agressivePathDestinationMultiplier = 10f;
    #endregion

    #region FLEE TARGET PARAMETERS
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    private float closestFleeTargetDistance = float.MaxValue;
    #endregion

    #region WOLF PARAMETERS
    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;
    private float agressiveFleeTimer;
    private float roamToAgressiveTimer;

    private float agressiveFleeTime;
    private float roamToAgressiveTime;
    private float agressiveTargetSheepTimeInterval;

    private float agressiveSpeed;
    private float fleeSpeed;
    private float roamSpeed;
    private float attackSpeed;
    private float agressiveMinDistanceToSheep;

    private float closestFleeTargetTriggerFleeDistance;
    private float closestFleeTargetStopDistance;
    private float closestFleeTargetDistanceToEatSheep;

    private float attackRange = 1.5f;
    private bool hasBitSheep;

    private int maxSheepInHerdToAttack;
    #endregion

    private void Awake() {
        state = State.Roam;

        roamPauseMaxTime = wolfSO.roamPauseMaxTime;
        roamPauseMinTime = wolfSO.roamPauseMinTime;
        roamToAgressiveTime = wolfSO.roamToAgressiveTime;
        agressiveFleeTime = wolfSO.agressiveFleeTime;

        agressiveTargetSheepTimeInterval = wolfSO.agressiveTargetSheepTimeInterval;
        agressiveSpeed = wolfSO.agressiveSpeed;
        fleeSpeed = wolfSO.fleeSpeed;
        roamSpeed = wolfSO.roamSpeed;
        attackSpeed = wolfSO.attackSpeed;

        maxSheepInHerdToAttack = wolfSO.maxSheepInHerdToAttack;

        agressiveMinDistanceToSheep = wolfSO.agressiveMinDistanceToSheep;

        closestFleeTargetTriggerFleeDistance = wolfSO.closestFleeTargetTriggerFleeDistance;
        closestFleeTargetStopDistance = wolfSO.closestFleeTargetStopDistance;
        closestFleeTargetDistanceToEatSheep = wolfSO.closestFleeTargetDistanceToEatSheep;
    }

    protected override void Start() {
        seeker = GetComponent<Seeker>();

        // Initialise sheep lists and arrays
        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();

        sheepsinLevel = new List<Sheep>();
        foreach (Sheep sheep in initialSheepsInLevel) {
            sheepsinLevel.Add(sheep);
        }

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
                moveSpeed = fleeSpeed;


                if (closestFleeTargetDistance > closestFleeTargetStopDistance) {
                    // Closest target is out of flee stop radius
                    state = State.Roam;
                    roamToAgressiveTimer = roamToAgressiveTime;
                    return;
                }
                else {
                    // Target is within flee stop radius
                    CalculateFleePath(closestFleeTarget.transform.position);
                }

                FollowPath(path);

                break;

            case State.Roam:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;
                roamToAgressiveTimer -= Time.deltaTime;

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    return;
                }

                if (roamToAgressiveTimer < roamToAgressiveTime) {
                    state = State.Agressive;
                    roamToAgressiveTimer = roamToAgressiveTime;
                    return;
                }

                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint(transform.position);
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                FollowPath(path);
                break;

            case State.Agressive:
                moveSpeed = agressiveSpeed;
                agressiveTargetSheepTimer -= Time.deltaTime;

                if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
                    // Target is within flee trigger radius
                    state = State.Flee;
                    return;
                }

                closestAttackTargetSheep = PickClosestAttackTargetSheep();
                if (closestAttackTargetSheep != null) {
                    state = State.Attack;
                    return;
                }

                if (agressiveTargetSheepTimer <= 0) {
                    PickAgressiveTargetSheep();
                }

                FindClosestSheep();

                if (distanceToClosestSheep < agressiveMinDistanceToSheep) {
                    // Wolf is too close to the sheep
                    CalculatePath(transform.position + -vectorToClosestSheep * fleeClosestSheepDestinationMultiplier);
                    state = State.AgressiveFlee;
                    agressiveFleeTimer = agressiveFleeTime;
                    return;
                }
                else {
                    // Calculate path to a point perpendicular to the sheep
                    Vector3 dirToSheepNormalized = (agressiveTargetSheep.transform.position - transform.position).normalized;
                    Vector3 perpendicularDirToSheepNormalized = new Vector3(dirToSheepNormalized.y, -dirToSheepNormalized.x, 0);
                    Vector3 perpendicularInverseDirToSheepNormalized = new Vector3(-dirToSheepNormalized.y, dirToSheepNormalized.x, 0);

                    if (inverseDirection) {
                        CalculatePath(transform.position + perpendicularDirToSheepNormalized * agressivePathDestinationMultiplier);
                    }
                    else {
                        CalculatePath(transform.position + perpendicularInverseDirToSheepNormalized * agressivePathDestinationMultiplier);
                    }
                }
                FollowPath(path);

                break;

            case State.AgressiveFlee:
                agressiveFleeTimer -= Time.deltaTime;
                FollowPath(path);

                if (agressiveFleeTimer <= 0) {
                    state = State.Agressive;
                    return;
                }

                break;

            case State.Attack:
                moveSpeed = attackSpeed;

                if (!hasBitSheep) {
                    // Wolf has not bit sheep yet
                    CalculatePath(closestAttackTargetSheep.transform.position);

                    // Check if sheep is in range
                    float distanceToClosestAttackTargetSheep = Vector3.Distance(transform.position, closestAttackTargetSheep.transform.position);
                    if (distanceToClosestAttackTargetSheep < attackRange) {
                        AttackSheep(closestAttackTargetSheep);
                        hasBitSheep = true;
                    }
                }
                else {
                    // Wolf has bit sheep
                    state = State.EatSheepFlee;
                    return;
                }
                FollowPath(path);
                break;

            case State.EatSheepFlee:
                moveSpeed = fleeSpeed;
                // Maintain sheep in mouth
                attackTargetSheep.transform.position = sheepHoldPoint.transform.position;

                CalculateFleePath(closestFleeTarget.transform.position);

                if (closestFleeTargetDistance > closestFleeTargetDistanceToEatSheep) {
                    EatSheep(attackTargetSheep);
                }

                FollowPath(path);
                break;
        }
    }

    private void AttackSheep(Sheep sheep) {
        attackTargetSheep = sheep;
        attackTargetSheep.BiteSheep();
        attackTargetSheep.SetSheepParent(sheepHoldPoint);
    }

    private void EatSheep(Sheep sheep) {
        Debug.Log("Sheep Eaten");
    }

    private Sheep PickRandomSheep() {
        int targetSheepindex = UnityEngine.Random.Range(0, sheepsinLevel.Count);
        return sheepsinLevel[targetSheepindex];
    }

    private Sheep PickClosestAttackTargetSheep() {
        closestAttackTargetSheep = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Sheep potentialSheep in sheepsinLevel) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            // Sheep surroundings
            int sheepNumberWithinTargetSheepRadius = potentialSheep.GetHerdNumber();

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0 & sheepNumberWithinTargetSheepRadius <= maxSheepInHerdToAttack) {
                closestDistanceSqr = dSqrToSheep;
                closestAttackTargetSheep = potentialSheep;
            }
        }
        return closestAttackTargetSheep;
    }

    private void PickAgressiveTargetSheep() {
        // Pick a new target Sheep (closest)
        agressiveTargetSheep = FindClosestSheep();
        agressiveTargetSheepTimer = agressiveTargetSheepTimeInterval;

        // Pick a random direction between + and -
        inverseDirection = UnityEngine.Random.value >= 0.5;
    }

    private Sheep FindClosestSheep() {

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Sheep potentialSheep in sheepsinLevel) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - transform.position;
            float dSqrToSheep = directionToSheep.sqrMagnitude;

            if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0) {
                closestDistanceSqr = dSqrToSheep;
                closestSheep = potentialSheep;
            }
        }

        vectorToClosestSheep = (closestSheep.transform.position - transform.position);
        distanceToClosestSheep = vectorToClosestSheep.magnitude;

        return closestSheep;
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

    public void AddFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Add(fleeTarget);
    }

}
