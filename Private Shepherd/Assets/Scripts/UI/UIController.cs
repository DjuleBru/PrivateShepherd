using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable() {
        StartCoroutine(SetButtonActive());
    }

    private IEnumerator SetButtonActive() {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstSelected);
        yield return null;
        }
}
