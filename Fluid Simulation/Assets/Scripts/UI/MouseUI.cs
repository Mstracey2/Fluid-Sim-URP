using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class MouseUI : SetSliderValue
{
    [SerializeField] private MouseParticleMover mouseMover;
    public UnityEvent mouseEvent;

    private void Start()
    {
        mouseEvent.Invoke();
    }

    public void UpdateSensitivity()
    {
        mouseMover.Sensitivity = slider.value;
    }

    public void UpdateRadius()
    {
        mouseMover.Radius(slider.value);
        SetValueFromSlider();
    }




}
