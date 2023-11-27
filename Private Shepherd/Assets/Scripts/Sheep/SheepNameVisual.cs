using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SheepNameVisual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sheepNameVisual;
    [SerializeField] BoxCollider2D nameCollider;
    [SerializeField] LayerMask sheepLayerMask;

    private List<string> levelSheepNames = new List<string>();
    private string sheepName;

    private int collisionNumber = 0;

    private void Start() {
        levelSheepNames = SheepNames.Instance.GetLevelSheepNamesList();

        int sheepNameInt = Random.Range(0, levelSheepNames.Count);
        sheepName = levelSheepNames[sheepNameInt];
        SheepNames.Instance.RemoveSheepNameFromList(sheepName);
        sheepNameVisual.text = sheepName;


        float colliderSizeX = sheepName.Length * .25f;
        nameCollider.size = new Vector3(colliderSizeX, nameCollider.size.y);
    }

    //private void Update() {
    //    if (nameCollider.IsTouchingLayers(sheepLayerMask)) {
    //        sheepNameVisual.text = "";
    //    } else {
    //        sheepNameVisual.text = sheepName;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.TryGetComponent<SheepNameVisual>(out SheepNameVisual sheepNames) | collision.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            collisionNumber++;
        }

        if (collisionNumber > 0) {
            sheepNameVisual.text = "";
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.TryGetComponent<SheepNameVisual>(out SheepNameVisual sheepNames) | collision.gameObject.TryGetComponent<Sheep>(out Sheep sheep)) {
            collisionNumber--;
        }
        if (collisionNumber == 0) {
            sheepNameVisual.text = sheepName;
        }
    }

}
