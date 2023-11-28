using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : AIMovement
{
    [SerializeField] private float roamSpeed;
    private float roamTimer;
    [SerializeField] private float roamRate;

    private void Awake() {
        roamTimer = roamRate;

        Vector3 roamTarget = PickRandomPoint(transform.position);
        moveSpeed = roamSpeed;
    }

    private void Update() {

        if (path == null | cutSceneInProgress) {
            reachedEndOfPath = true;
            return;
        }

        roamTimer -= Time.deltaTime;
        if(roamTimer < 0) {
            roamTimer = roamRate;
            Vector3 roamTarget = PickRandomPoint(transform.position);
            CalculatePath(roamTarget);
        }

        FollowPath(path);
    }


}
