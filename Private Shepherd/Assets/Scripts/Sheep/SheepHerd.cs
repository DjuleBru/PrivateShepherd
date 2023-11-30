using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SheepHerd : MonoBehaviour
{
    private List<Sheep> herd;

    [SerializeField] float herdRadius = 5f;
    [SerializeField] LayerMask sheepLayerMask;
    [SerializeField] private int sheepLayerMaskInt;
    [SerializeField] CircleCollider2D herdCollider;
    [SerializeField] TextMeshPro herdNumberText;

    [SerializeField] bool isUniqueHerd;


    private int herdNumber;

    private void Awake() {
        herd = new List<Sheep>();
        herdCollider.radius = herdRadius;
    }

    private void Update() {
        herdNumber = GetHerdNumber();
        herdNumberText.text = herdNumber.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(isUniqueHerd) {
            if (collider.gameObject.TryGetComponent<Sheep>(out Sheep sheep) & collider.gameObject.layer == sheepLayerMaskInt) {
                herd.Add((Sheep)sheep);
            }
        }
        else
        {
            if (collider.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
                herd.Add((Sheep)sheep);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if(isUniqueHerd) {
            if (collider.gameObject.TryGetComponent<Sheep>(out Sheep sheep) & collider.gameObject.layer == sheepLayerMaskInt) {
                herd.Remove((Sheep)sheep);
            }
        } else {
            if (collider.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
                herd.Remove((Sheep)sheep);
            }
        }
    }

    public Sheep GetSheepInHerdFleeing() {
        Sheep sheepParent = GetComponentInParent<Sheep>();

        if(sheepParent.GetSheepType() == SheepType.goat) {
            // This sheep is a goat
            return null;
        }

        // Check if there is a goat in the herd
        foreach (Sheep sheep in herd) {
            if (sheep.GetComponent<Sheep>().GetSheepType() == SheepType.goat) {
                // There is a goat in the herd
                return sheep;
            }
        }

        foreach (Sheep sheep in herd) {
            if (sheep.GetComponent<SheepMovement>().GetState() == SheepMovement.State.Flee) {
                return sheep;
            }
        }
        return null;
    }


    public int GetHerdNumber() {
        return herd.Count;
    }

    public List<Sheep> GetHerd() {
        return herd;
    }

}
