using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tentacle : MonoBehaviour
{
    [SerializeField] private SheepObjectPool levelSheepObjectPool;
    [SerializeField] private Transform sheepHoldPoint;


    private State state;
    public enum State {
        Immerged,
        Emerged,
    }

    public event EventHandler OnSheepBite;
    public event EventHandler OnSheepEaten;
    public event EventHandler OnTentacleEmerge;

    #region TARGET SHEEP PARAMETERS
    [Header("TARGET SHEEP PARAMETERS")]
    [SerializeField] LayerMask sheepLayerMask;
    private List<Sheep> targetableSheepsinLevel;
    private Sheep[] initialSheepsInLevel;
    private Sheep closestSheep;

    private Sheep closestAttackTargetSheep;
    private Sheep childTargetSheep;
    #endregion

    [SerializeField] private float emergeDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackRate;
    [SerializeField] private float firstAttackDelay;
    [SerializeField] private float sheepHoldTime;
    private float sheepHoldTimer;
    private float attackAnimationHalfDuration = .2f;

    private float attackTimer;
    private bool hasBitSheep;
    private bool isCarryingSheep;

    void Start()
    {
        attackTimer = firstAttackDelay;
        sheepHoldTimer = sheepHoldTime;
        state = State.Immerged;

        targetableSheepsinLevel = new List<Sheep>();
        // Initialise sheep lists and arrays
        initialSheepsInLevel = levelSheepObjectPool.GetSheepArray();
        foreach (Sheep sheep in initialSheepsInLevel) {
            targetableSheepsinLevel.Add(sheep);
        }

        levelSheepObjectPool.OnSheepDied += LevelSheepObjectPool_OnSheepDied;
        levelSheepObjectPool.OnSheepPenned += LevelSheepObjectPool_OnSheepPenned;
    }

    void Update()
    {
        if (targetableSheepsinLevel.Count == 0) {
            // All sheep are dead
            return;
        }

        closestSheep = FindClosestSheep();
        float distanceToClosestSheep = Vector3.Distance(closestSheep.transform.position, transform.position);

        switch (state) {
            case State.Immerged:
                if (Vector3.Distance(closestSheep.transform.position, transform.position) < emergeDistance) {
                    OnTentacleEmerge?.Invoke(this, EventArgs.Empty);
                    state = State.Emerged;
                }
                break;

            case State.Emerged:
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0 & distanceToClosestSheep < attackDistance) {
                    if (!hasBitSheep) {
                        StartCoroutine(BiteSheep(closestSheep));
                    } else {
                        if (!isCarryingSheep) {
                            return;
                        }
                        else {
                            // Wolf has finished attack animation and is carrying sheep
                            sheepHoldTimer -= Time.deltaTime;
                            if(sheepHoldTimer < 0) {
                                StartCoroutine(EatSheep(childTargetSheep));
                                attackTimer = attackRate;
                                sheepHoldTimer = sheepHoldTime;
                                hasBitSheep = false;
                                return;
                            }
                        }
                    }
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
        Debug.Log("sheep eaten");
        OnSheepEaten?.Invoke(this, EventArgs.Empty);

        yield return new WaitForSeconds(attackAnimationHalfDuration);

        sheep.EatSheep();
        isCarryingSheep = false;
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

        return closestSheep;
    }


    private void LevelSheepObjectPool_OnSheepDied(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }

    private void LevelSheepObjectPool_OnSheepPenned(object sender, EventArgs e) {
        targetableSheepsinLevel = levelSheepObjectPool.GetSheepsInUnPennedObjectPoolList();
    }

}
