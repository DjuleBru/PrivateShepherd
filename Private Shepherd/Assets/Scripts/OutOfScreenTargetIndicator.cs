using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfScreenTargetIndicator : MonoBehaviour
{
    [SerializeField] private GameObject outOfScreenGameObject;
    private GameObject outOfScreenPointer;
    private Camera mainCamera;
    private bool pointerIsActive;

    private void Awake() {
        mainCamera = Camera.main;
    }

    private void Start() {
        outOfScreenPointer = Instantiate(outOfScreenGameObject, transform.position, transform.rotation);
        outOfScreenPointer.SetActive(false);
    }


    private void Update() {
        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(transform.position);

        if (viewPortPosition.x > 0 && viewPortPosition.x < 1 && viewPortPosition.y > 0 && viewPortPosition.y < 1) {
            // Target is on screen, hide the indicator
            outOfScreenPointer.SetActive(false);
        }
        else
        {
            // Target is off screen, activate the indicator
            outOfScreenPointer.SetActive(true);
            Vector3 screenEdge = mainCamera.ViewportToWorldPoint(new Vector3(Mathf.Clamp(viewPortPosition.x, .1f, 0.9f), Mathf.Clamp(viewPortPosition.y, .1f, 0.9f), mainCamera.nearClipPlane));
            outOfScreenPointer.transform.position = new Vector3(screenEdge.x, screenEdge.y, 0f);

            Vector3 direction = mainCamera.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            outOfScreenPointer.transform.rotation = Quaternion.Euler(0,0,angle - 90);
        }
    }

    public void OnHolderDestroyed() {
        Destroy(outOfScreenPointer);
    }

}
