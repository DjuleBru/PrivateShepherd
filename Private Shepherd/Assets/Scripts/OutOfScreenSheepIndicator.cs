using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenSheepIndicator : OutOfScreenTargetIndicator
{

   [SerializeField] private Sheep sheep;
   [SerializeField] private LayerMask sheepLayerMask;
   private List<Sheep> herd;
   private Collider2D[] sheepInRadius;
   private float radius = 10f;

   List<Collider2D> sheepInRadiusList;
    private float minPointerScale = 1f;
    private float maxPointerScale = 2f; 

    private float pointerScale = 1f;

    protected override void Update() {
        if (!pointerExternalActivator) {
            return;
        }

        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x > 0 && viewPortPosition.x < 1 && viewPortPosition.y > 0 && viewPortPosition.y < 1) {
            // Target is on screen, hide the indicator
            isOutOfScreen = false;
            outOfScreenPointer.SetActive(false);
            pointerInternalActive = false;
        }
        else {
            isOutOfScreen = true;

            // Check if other sheep are in herd and have indicator active
            sheepInRadius = Physics2D.OverlapCircleAll(transform.position, radius, sheepLayerMask);
            sheepInRadiusList = new List<Collider2D>();

            // Sheep is in a herd
            foreach (Collider2D collider in sheepInRadius) {
                if (collider.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
                    // Collider is the sheep collider
                    if (collider.gameObject != this.gameObject) {
                        // Collider is not this gameObject
                        sheepInRadiusList.Add(collider);
                    }
                }
            }

            foreach (Collider2D sheepCollider in sheepInRadiusList) {
                if (sheepCollider.gameObject.TryGetComponent<OutOfScreenTargetIndicator>(out OutOfScreenTargetIndicator outOfScreenTargetIndicator)) {
                    bool pointerIsActiveInHerd = outOfScreenTargetIndicator.GetInternalPointerIsActive();
                    
                    if (pointerIsActiveInHerd) {
                        // There is one sheep in herd with target indicator active and this sheep's pointer is active
                        outOfScreenPointer.SetActive(false);
                        pointerInternalActive = false;
                        return;
                    }

                }
            }

            // Activate the indicator
            pointerInternalActive = true;
            outOfScreenPointer.SetActive(true);

            // Change Indicator distance to screen border with distance to player
            CalculateDistanceScreenEdge();

            Vector3 screenEdge = mainCamera.ViewportToWorldPoint(new Vector3(Mathf.Clamp(viewPortPosition.x, 0.1f, 0.85f), Mathf.Clamp(viewPortPosition.y, 0.1f, 0.85f), mainCamera.nearClipPlane));
            outOfScreenPointer.transform.position = new Vector3(screenEdge.x + distanceScreenEdgex, screenEdge.y + distanceScreenEdgey, 0f);

            Vector3 direction = mainCamera.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            outOfScreenPointer.transform.rotation = Quaternion.Euler(0, 0, angle - 90);


            // Change Indicator size with sheep surrounding number
            int isOutOfScreenNumber = GetSheepInRadiusOutsideOfScreenNumber();

            pointerScale = minPointerScale + (float)isOutOfScreenNumber / 10;
            if (pointerScale > maxPointerScale) {
                pointerScale = maxPointerScale;
            }
            outOfScreenPointer.transform.localScale = new Vector3(pointerScale, pointerScale, 1);

        }
    }


    private int GetSheepInRadiusOutsideOfScreenNumber() {
        int isOutOfScreenNumber = 0;

        foreach (Collider2D sheepCollider in sheepInRadiusList) {
            if (sheepCollider.gameObject.TryGetComponent<OutOfScreenTargetIndicator>(out OutOfScreenTargetIndicator outOfScreenTargetIndicator)) {
                bool isOutOfScreen = outOfScreenTargetIndicator.GetIsOutOfScreen();

                if (isOutOfScreen) {
                    // There is one sheep in herd with target indicator active
                    isOutOfScreenNumber++;
                }
            }
        }
        return isOutOfScreenNumber;
    }

}
