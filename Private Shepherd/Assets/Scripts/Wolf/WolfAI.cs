using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class WolfAI : AIMovement {

    [SerializeField] private WolfSO wolfSO;
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private Transform sheepHoldPoint;

    public event EventHandler OnSheepBite;
    public event EventHandler OnSheepEaten;

    private bool inverseDirection;

    private State state;
    public enum State {
        Flee,
        Roam,
        Agressive,
        AgressiveFlee,
        Attack,
        FleeWithSheep,
    }

    #region TARGET SHEEP PARAMETERS
    [SerializeField] LayerMask sheepLayerMask;
    private List<Sheep> sheepsinLevel;
    private Sheep[] initialSheepsInLevel;
    private Sheep closestSheep;

    private Sheep closestAttackTargetSheep;
    private Sheep childTargetSheep;

    private Sheep agressiveTargetSheep;
    private Vector3 vectorToClosestSheep;
    private float distanceToClosestSheep;

    float fleeClosestSheepDestinationMultiplier = 3f;
    float agressivePathDestinationMultiplier = 10f;
    private int randomDirSelectorCount;
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
    private float agressiveTargetSheepRate;

    private float agressiveSpeed;
    private float fleeSpeed;
    private float roamSpeed;
    private float attackSpeed;
    private float agressiveMinDistanceToSheep;
    private float agressiveMaxDistanceToSheep;

    private float wolfTriggerFleeDistanceMultiplier;
    private float closestFleeTargetTriggerFleeDistance;
    private float closestFleeTargetStopDistance;
    private float closestFleeTargetDistanceToEatSheep;

    private float attackRange = 1.75f;
    private bool hasBitSheep;
    private bool isCarryingSheep;

    private int maxSheepInHerdToAttack;

    private float attackAnimationHalfDuration = .2f;
    #endregion

    private void Awake() {
        state = State.Roam;

        roamPauseMaxTime = wolfSO.roamPauseMaxTime;
        roamPauseMinTime = wolfSO.roamPauseMinTime;
        roamToAgressiveTime = wolfSO.roamToAgressiveTime;
        agressiveFleeTime = wolfSO.agressiveFleeTime;

        agressiveTargetSheepRate = wolfSO.agressiveTargetSheepRate;
        agressiveSpeed = wolfSO.agressiveSpeed;
        fleeSpeed = wolfSO.fleeSpeed;
        roamSpeed = wolfSO.roamSpeed;
        attackSpeed = wolfSO.attackSpeed;

        maxSheepInHerdToAttack = wolfSO.maxSheepInHerdToAttack;

        agressiveMinDistanceToSheep = wolfSO.agressiveMinDistanceToSheep;
        agressiveMaxDistanceToSheep = wolfSO.agressiveMaxDistanceToSheep;

        closestFleeTargetTriggerFleeDistance = wolfSO.closestFleeTargetTriggerFleeDistance;
        closestFleeTargetStopDistance = wolfSO.closestFleeTargetStopDistance;
        closestFleeTargetDistanceToEatSheep = wolfSO.closestFleeTargetDistanceToEatSheep;
        wolfTriggerFleeDistanceMultiplier = wolfSO.wolfTriggerFleeDistanceMultiplier;
    }

    protected override void Start() {
        seeker = GetComponent<Seeker>();

        levelSheepObjectPool.OnSheepRemoved += LevelSheepObjectPool_OnSheepRemoved;

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

        FollowPath(path);

        closestFleeTarget = FindClosestFleeTarget();
        UpdateClosestFleeParameters();

        if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance) {
            // Target is within flee trigger radius
            state = State.Flee;
        }

        closestAttackTargetSheep = PickClosestAttackTargetSheep();

        switch (state) {
            case State.Flee:
                moveSpeed = fleeSpeed;

                if (isCarryingSheep) {
                    DropSheep();
                }

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

                break;

            case State.Roam:
                moveSpeed = roamSpeed;
                roamPauseTimer -= Time.deltaTime;
                roamToAgressiveTimer -= Time.deltaTime;

                if (roamToAgressiveTimer < 0) {
                    state = State.Agressive;
                    return;
                }

                if (roamPauseTimer <= 0) {
                    Vector3 roamDestination = PickRandomPoint(transform.position);
                    CalculatePath(roamDestination);
                    roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
                }

                break;

            case State.Agressive:
                moveSpeed = agressiveSpeed;

                if (closestAttackTargetSheep != null) {
                    state = State.Attack;
                    return;
                }

                PickAgressiveTargetSheep();
                FindClosestSheep();

                if (distanceToClosestSheep < agressiveMinDistanceToSheep) {
                    // Wolf is too close to the sheep
                    CalculatePath(transform.position + -vectorToClosestSheep * fleeClosestSheepDestinationMultiplier);
                    state = State.AgressiveFlee;
                    agressiveFleeTimer = agressiveFleeTime;
                    return;
                }

                if (distanceToClosestSheep > agressiveMaxDistanceToSheep) {
                    // Wolf is too far from sheep
                    CalculatePath(closestSheep.transform.position);
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

                if (closestAttackTargetSheep == null) {
                    state = State.Agressive;
                    return;
                }

                if (!hasBitSheep) {
                    // Wolf has not bit sheep yet
                    CalculatePath(closestAttackTargetSheep.transform.position);

                    // Check if sheep is in range
                    float distanceToClosestAttackTargetSheep = Vector3.Distance(transform.position, closestAttackTargetSheep.transform.position);
                    if (distanceToClosestAttackTargetSheep < attackRange & !closestAttackTargetSheep.GetHasBeenBit()) {
                        StartCoroutine(BiteSheep(closestAttackTargetSheep));
                    }
                }
                else {
                    // Wolf attack animation playing
                    if (!isCarryingSheep) {
                        return;
                    }
                    else {
                        // Wolf has finished attack animation and is carrying sheep
                        state = State.FleeWithSheep;
                        hasBitSheep = false;
                        return;
                    }
                }

                break;

            case State.FleeWithSheep:
                moveSpeed = fleeSpeed;
                // Maintain sheep in mouth
                childTargetSheep.transform.position = sheepHoldPoint.transform.position;

                CalculateFleePath(closestFleeTarget.transform.position);

                if (closestFleeTargetDistance > closestFleeTargetDistanceToEatSheep) {
                    StartCoroutine(EatSheep(childTargetSheep));

                    state = State.Roam;
                    roamToAgressiveTimer = roamToAgressiveTime;
                    return;
                }

                break;
        }
    }

    private IEnumerator BiteSheep(Sheep sheep) {
        hasBitSheep = true;
        OnSheepBite?.Invoke(this, EventArgs.Empty);

        childTargetSheep = sheep;
        childTargetSheep.BiteSheep();

        yield return new WaitForSeconds(attackAnimationHalfDuration);

        childTargetSheep.SetSheepParent(sheepHoldPoint);

        isCarryingSheep = true;
    }

    private IEnumerator EatSheep(Sheep sheep) {
        OnSheepEaten?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(attackAnimationHalfDuration);

        sheep.EatSheep();
        isCarryingSheep = false;
    }

    private void DropSheep() {
        childTargetSheep.DropSheep();
        childTargetSheep = null;
        isCarryingSheep = false;
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

        randomDirSelectorCount++;

        // Pick a random direction between + and -
        int randomDirSelectorMaxCount = 40;
        if (randomDirSelectorCount > randomDirSelectorMaxCount) {
            inverseDirection = UnityEngine.Random.value >= 0.5;
            randomDirSelectorCount = 0;
        }
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

    private void UpdateClosestFleeParameters() {
        closestFleeTargetDistance = Vector3.Distance(closestFleeTarget.transform.position, transform.position);
        closestFleeTargetTriggerFleeDistance = closestFleeTarget.GetFleeTargetTriggerDistance() * wolfTriggerFleeDistanceMultiplier;
    }

    public void AddFleeTarget(FleeTarget fleeTarget) {
        fleeTargetList.Add(fleeTarget);
    }

    private void LevelSheepObjectPool_OnSheepRemoved(object sender, EventArgs e) {
         sheepsinLevel = levelSheepObjectPool.GetSheepsInObjectPoolList();
    }
}
