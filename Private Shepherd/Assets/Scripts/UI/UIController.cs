using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable() {
        Debug.Log("UI Enabled");
        StartCoroutine(SetButtonActive());
    }

    private IEnumerator SetButtonActive() {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstSelected);
        yield return null;
        }
}
