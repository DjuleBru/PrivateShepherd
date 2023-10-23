using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {

    public event EventHandler OnButtonSelected;
    public event EventHandler OnButtonDeSelected;

    public void OnSelect(BaseEventData eventData) {
        OnButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnDeselect(BaseEventData eventData) {
        OnButtonDeSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnButtonSelected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnButtonDeSelected?.Invoke(this, EventArgs.Empty);
    }
}
