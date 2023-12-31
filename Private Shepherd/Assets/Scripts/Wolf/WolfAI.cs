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

    [SerializeField] private float wolfPoisonedTime;

    public event EventHandler OnSheepBite;
    public event EventHandler OnSheepEaten;
    public event EventHandler OnWolfFlee;
    public event EventHandler OnWolfAgressive;
    public event EventHandler OnWolfPoisoned;

    public static event EventHandler OnAnySheepBite;
    public static event EventHandler OnAnySheepEaten;
    public static event EventHandler OnAnyWolfFlee;

    public event EventHandler<OnWolfDiedEventArgs> OnWolfDied;

    public class OnWolfDiedEventArgs : EventArgs {
        public FleeTarget fleeTarget;
    }

    private bool inverseDirection;
    private bool dyingAnimation;

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
    [Header("TARGET SHEEP PARAMETERS")]
    [SerializeField] LayerMask sheepLayerMask;
    private List<Sheep> targetableSheepsinLevel;
    private List<Sheep> redSheepsInLevel;
    private Sheep[] initialSheepsInLevel;
    private Sheep closestSheep;

    private Sheep closestAttackTargetSheep;
    private Sheep childTargetSheep;

    private Sheep agressiveTargetSheep;
    private Vector3 vectorToClosestSheep;
    private float distanceToClosestSheep;

    private float fleeClosestSheepDestinationMultiplier = 5f;
    private float agressivePathDestinationMultiplier = 10f;
    private float randomDirSelectorTimer;
    [SerializeField] private float randomDirSelectorTime = 5;
    #endregion

    #region FLEE TARGET PARAMETERS
    [Header("FLEE TARGET PARAMETERS")]
    private List<FleeTarget> fleeTargetList = new List<FleeTarget>();
    private FleeTarget closestFleeTarget;
    private float closestFleeTargetDistance = float.MaxValue;
    #endregion

    #region WOLF PARAMETERS
    [Header("WOLF PARAMETERS")]
    private float roamPauseMaxTime;
    private float roamPauseMinTime;
    private float roamPauseTimer;
    private float agressiveFleeTimer;
    private float roamToAgressiveTimer;

    private float agressiveFleeTime;
    private float roamToAgressiveTime;

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

    private float attackRange = 2f;
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

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;
        levelSheepObjectPool.OnSheepPenned += LevelSheepObjectPool_OnSheepPenned;

        // Initialise sheep lists and arrays
        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();

        targetableSheepsinLevel = new List<Sheep>();
        redSheepsInLevel = new List<Sheep>();
        foreach (Sheep sheep in initialSheepsInLevel) {
            targetableSheepsinLevel.Add(sheep);
            if(sheep.GetSheepType() == SheepType.redSheep) {
                redSheepsInLevel.Add(sheep);
            }
        }

        //Initialise path
        CalculatePath(transform.position);
        roamPauseTimer = UnityEngine.Random.Range(roamPauseMinTime, roamPauseMaxTime);
        randomDirSelectorTimer = randomDirSelectorTime;
        nextWaypointDistance = 0.5f;

        fleeTargetList.Add(PlayerGrowl.Instance.GetGrowlFleeTarget());
    }


    private void Update() {

        if (path == null | dyingAnimation) {
            return;
        }

        if (targetableSheepsinLevel.Count == 0) {
            // All sheep are dead
            return;
        }

        FollowPath(path);

        closestFleeTarget = FindClosestFleeTarget();
        UpdateClosestFleeParameters();

        if (closestFleeTargetDistance <= closestFleeTargetTriggerFleeDistance & state != State.Flee) {
            // Target is within flee trigger radius
            state = State.Flee;
            OnWolfFlee?.Invoke(this, EventArgs.Empty);
            OnAnyWolfFlee?.Invoke(this, EventArgs.Empty);
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
                    roamToAgressiveTimer = roamToAgressiveTime;
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
                randomDirSelectorTimer -= Time.deltaTime;

                if (closestAttackTargetSheep != null) {
                    state = State.Attack;
                    return;
                }

                if (randomDirSelectorTimer >= 0) {
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
        OnAnySheepBite?.Invoke(this, EventArgs.Empty);

        childTargetSheep = sheep;
        childTargetSheep.BiteSheep();

        yield return new WaitForSeconds(attackAnimationHalfDuration);

        childTargetSheep.SetSheepParent(sheepHoldPoint);

        isCarryingSheep = true;
    }

    private IEnumerator EatSheep(Sheep sheep) {
        Debug.Log("sheep eaten");
        OnSheepEaten?.Invoke(this, EventArgs.Empty);
        OnAnySheepEaten?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(attackAnimationHalfDuration);

        if (sheep.GetSheepType() == SheepType.greenSheep) {
            StartCoroutine(Die());
        }
        sheep.EatSheep();
        isCarryingSheep = false;
    }

    private void DropSheep() {
        childTargetSheep.DropSheep();
        childTargetSheep = null;
        isCarryingSheep = false;
    }

    private Sheep PickClosestAttackTargetSheep() {
        closestAttackTargetSheep = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if(redSheepsInLevel.Count > 0) {
            foreach(Sheep potentialRedSheep in  redSheepsInLevel) {

                // Distance to sheep
                Vector3 directionToSheep = potentialRedSheep.transform.position - currentPosition;
                float dSqrToSheep = directionToSheep.sqrMagnitude;

                if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0) {
                    closestDistanceSqr = dSqrToSheep;
                    closestAttackTargetSheep = potentialRedSheep;
                }
            }
            return closestAttackTargetSheep;
        }

        foreach (Sheep potentialSheep in targetableSheepsinLevel) {

            // Distance to sheep
            Vector3 directionToSheep = potentialSheep.transform.position - currentPosition;
            float dSqrToSheep = directionToSheep.sqrMagnitude;


            // Sheep surroundings
            int sheepNumberWithinTargetSheepRadius = potentialSheep.GetComponentInChildren<SheepHerd>().GetHerdNumber();


            if (potentialSheep.GetComponent<SheepMovement>().GetState() != SheepMovement.State.InScoreZone) {
                // Sheep is not in ScoreZone

                if (dSqrToSheep < closestDistanceSqr & dSqrToSheep != 0 & sheepNumberWithinTargetSheepRadius <= maxSheepInHerdToAttack) {
                    closestDistanceSqr = dSqrToSheep;
                    closestAttackTargetSheep = potentialSheep;
                }
            }
        }
        return closestAttackTargetSheep;
    }

    private void PickAgressiveTargetSheep() {
        // Pick a new target Sheep (closest)
        agressiveTargetSheep = FindClosestSheep();

        // Pick a random sheep and direction between + and -
        inverseDirection = UnityEngine.Random.value >= 0.5;
        randomDirSelectorTimer = 0;
    }

    private Sheep FindClosestSheep() {

        float closestDistanceSqr = Mathf.Infinity;

        foreach (Sheep potentialSheep in targetableSheepsinLevel) {

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
        FleeTarget closestFleeTarget = fleeTargetList[0];

        foreach (FleeTarget fleeTarget in fleeTargetList) {
            fleeTargetDistance = Vector3.Distance(fleeTarget.transform.position, transform.position);

            if (fleeTargetDistance <= closestFleeTargetDistance) {
                // The flee target is the closest one

                if(fleeTarget.GetFleeTargetTriggerDistance() > closestFleeTarget.GetFleeTargetTriggerDistance()) {
                    closestFleeTargetDistance = fleeTargetDistance;
                    closestFleeTarget = fleeTarget;
                }

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

    private void LevelSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
         targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
        redSheepsInLevel = new List<Sheep>();
        foreach (Sheep sheep in targetableSheepsinLevel) {
            if (sheep.GetSheepType() == SheepType.redSheep) {
                redSheepsInLevel.Add(sheep);
            }
        }
        PickAgressiveTargetSheep();
    }

    private void LevelSheepObjectPool_OnSheepPenned(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
        redSheepsInLevel = new List<Sheep>();
        foreach (Sheep sheep in targetableSheepsinLevel) {
            if (sheep.GetSheepType() == SheepType.redSheep) {
                redSheepsInLevel.Add(sheep);
            }
        }
        PickAgressiveTargetSheep();
    }

    private IEnumerator Die() {
        OnWolfPoisoned?.Invoke(this, EventArgs.Empty);

        float dieAnimationTime = 1f;
        yield return new WaitForSeconds(wolfPoisonedTime);


        reachedEndOfPath = true;
        path = null;
        dyingAnimation = true;
        OnWolfDied?.Invoke(this, new OnWolfDiedEventArgs {
            fleeTarget = GetComponent<FleeTarget>()
        });

        levelSheepObjectPool.OnSheepDied -= LevelSheepObjectPool_OnSheepDied;
        levelSheepObjectPool.OnSheepPenned -= LevelSheepObjectPool_OnSheepPenned;

        yield return new WaitForSeconds(dieAnimationTime);
        Destroy(gameObject);
    }

    public State GetState() {
        return state;
    }

    public void SetState(State state) {
        this.state = state;
    }

}
