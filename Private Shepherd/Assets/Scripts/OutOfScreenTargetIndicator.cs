using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenTargetIndicator : MonoBehaviour
{
    [SerializeField] protected GameObject outOfScreenGameObject;
    protected GameObject outOfScreenPointer;
    protected Camera mainCamera;
    protected bool pointerExternalActivator;
    protected bool pointerInternalActive;
    protected bool isOutOfScreen;

    [SerializeField] protected float distanceScreenEdgex;
    [SerializeField] protected float distanceScreenEdgey;
    protected float maxDistanceScreenEdgex = 5.5f;
    protected float minDistanceScreenEdgex = 3.5f;

    protected float maxDistanceScreenEdgey = 2.5f;
    protected float minDistanceScreenEdgey = 1.5f;

    protected void Awake() {
        mainCamera = Camera.main;
    }

    protected void Start() {
        outOfScreenPointer = Instantiate(outOfScreenGameObject, transform.position, transform.rotation);
        outOfScreenPointer.SetActive(false);
        pointerExternalActivator = true;
    }


    protected virtual void Update() {

        if (!pointerExternalActivator) {
            return;
        }

        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x > 0 && viewPortPosition.x < 1 && viewPortPosition.y > 0 && viewPortPosition.y < 1) {

            isOutOfScreen = false;

            // Target is on screen, hide the indicator
            outOfScreenPointer.SetActive(false);
            pointerInternalActive = false;
        }
        else {
            isOutOfScreen = true;

            // Activate the indicator
            outOfScreenPointer.SetActive(true);
            pointerInternalActive = true;


            // Change Indicator distance to screen border with distance to player
            CalculateDistanceScreenEdge();

            // Change Indicator distance to screen border with distance to player
            Vector3 direction = mainCamera.transform.position - transform.position;
            float distanceToPlayer = new Vector3(direction.x, direction.y,0).magnitude;

            Vector3 screenEdge = mainCamera.ViewportToWorldPoint(new Vector3(Mathf.Clamp(viewPortPosition.x, .1f, 0.9f), Mathf.Clamp(viewPortPosition.y, .1f, 0.9f), mainCamera.nearClipPlane));
            outOfScreenPointer.transform.position = new Vector3(screenEdge.x, screenEdge.y, 0f);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            outOfScreenPointer.transform.rotation = Quaternion.Euler(0,0,angle - 90);
        }
    }
    protected void CalculateDistanceScreenEdge() {
        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(transform.position);

        float viewPortPositionMultiplierx = 8f;
        float viewPortPositionMultipliery = 3f;
        //X
        if (viewPortPosition.x > 1) {
            distanceScreenEdgex = maxDistanceScreenEdgex - ((viewPortPosition.x - 1) * viewPortPositionMultiplierx);
            if (distanceScreenEdgex > maxDistanceScreenEdgex) {
                distanceScreenEdgex = maxDistanceScreenEdgex;
            }
            if (distanceScreenEdgex < 0) {
                distanceScreenEdgex = 0;
            }
        }
        else if (viewPortPosition.x < 0) {
            distanceScreenEdgex = -(minDistanceScreenEdgex + ((viewPortPosition.x) * viewPortPositionMultiplierx));
            if (distanceScreenEdgex > 0) {
                distanceScreenEdgex = 0;
            }
            if (distanceScreenEdgex < -minDistanceScreenEdgex) {
                distanceScreenEdgex = -minDistanceScreenEdgex;
            }
        }
        else if (viewPortPosition.x >= 0 & viewPortPosition.x <= 1) {
            distanceScreenEdgex = 0;
        }
        //Y
        if (viewPortPosition.y > 1) {
            distanceScreenEdgey = maxDistanceScreenEdgey - ((viewPortPosition.y - 1) * viewPortPositionMultipliery);
            if (distanceScreenEdgey > maxDistanceScreenEdgey) {
                distanceScreenEdgey = maxDistanceScreenEdgey;
            }
            if (distanceScreenEdgey < 0) {
                distanceScreenEdgey = 0;
            }
        }
        else if (viewPortPosition.y < 0) {
            distanceScreenEdgey = -(minDistanceScreenEdgey + ((viewPortPosition.y) * viewPortPositionMultipliery));
            if (distanceScreenEdgey > 0) {
                distanceScreenEdgey = 0;
            }
            if (distanceScreenEdgey < -minDistanceScreenEdgey) {
                distanceScreenEdgey = -minDistanceScreenEdgey;
            }
        }
        else if (viewPortPosition.y >= 0 & viewPortPosition.y <= 1) {
            distanceScreenEdgey = 0;
        }

    }

    public void DestroyIndicator() {
        Destroy(outOfScreenPointer);
    }

    public void DeActivateIndicator() {
        pointerExternalActivator = false;
    }

    public void ActivateIndicator() {
        pointerExternalActivator = true;
    }

    public bool GetInternalPointerIsActive() {
        return pointerInternalActive;
    }

    public bool GetIsOutOfScreen() {
        return isOutOfScreen;
    }

}
