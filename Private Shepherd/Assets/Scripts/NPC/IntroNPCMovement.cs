using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroNPCMovement : AIMovement {

    [SerializeField] private float roamSpeed;
    private float roamTimer;
    [SerializeField] private float roamRate;

    private bool roaming;

    private void Awake() {
        roamTimer = roamRate;

        Vector3 roamTarget = PickRandomPoint(transform.position);
        moveSpeed = roamSpeed;
        reachedEndOfPath = true;
        
    }

    private void Update() {

        if (path == null | cutSceneInProgress) {
            reachedEndOfPath = true;
            return;
        }

        if(roaming) {
            roamTimer -= Time.deltaTime;
            if (roamTimer < 0) {
                roamTimer = roamRate;
                Vector3 roamTarget = PickRandomPoint(transform.position);
                CalculatePath(roamTarget);
            }

        } else {
            CalculatePath(Player.Instance.transform.position);
        }

        FollowPath(path);
    }
}
