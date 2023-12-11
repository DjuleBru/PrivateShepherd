using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {

    public event EventHandler OnButtonSelected;
    public event EventHandler OnButtonDeSelected;

    public static event EventHandler OnAnyButtonSelected;
    public static event EventHandler OnAnyButtonDeSelected;
    public static event EventHandler OnAnyButtonClick;

    private Button button;

    void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        OnAnyButtonClick?.Invoke(this, EventArgs.Empty);
    }

    public void OnSelect(BaseEventData eventData) {
        OnButtonSelected?.Invoke(this, EventArgs.Empty);
        OnAnyButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnDeselect(BaseEventData eventData) {
        OnButtonDeSelected?.Invoke(this, EventArgs.Empty);
        OnAnyButtonDeSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnButtonSelected?.Invoke(this, EventArgs.Empty);
        OnAnyButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnButtonDeSelected?.Invoke(this, EventArgs.Empty);
        OnAnyButtonDeSelected?.Invoke(this, EventArgs.Empty);
    }
}
