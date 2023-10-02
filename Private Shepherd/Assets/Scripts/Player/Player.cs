using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(PlayerBark))]

public class Player : MonoBehaviour
{
    public event EventHandler OnPlayerMovedOverTreshold;

    public static Player Instance { get; private set; }

    private Vector3 newPosition;
    private Vector3 oldPosition;

    private float pathfindingRefreshDistanceTreshold = 1f;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        newPosition = transform.position;
        oldPosition = transform.position;
    }

    private void Update() {
        newPosition = transform.position;

        if (Vector3.Distance(newPosition, oldPosition) > pathfindingRefreshDistanceTreshold) {
            OnPlayerMovedOverTreshold?.Invoke(this, EventArgs.Empty);
            oldPosition = transform.position;
        }
    }

}
