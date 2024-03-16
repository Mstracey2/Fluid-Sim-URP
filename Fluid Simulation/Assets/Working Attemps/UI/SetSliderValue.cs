using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SPH;

public class SetSliderValue : MonoBehaviour
{
    protected Slider slider;
    public SPH.GPUVariables variable;
    [SerializeField] private SPH smoothParticleHydro;
    

    private void Awake()
    {
        slider = GetComponent<Slider>();
        SetValueFromSlider();
    }

    public void SetValueFromSlider()
    {
        string enumValueString = Enum.GetName(typeof(SPH.GPUVariables), variable);
        smoothParticleHydro.SetGpuFloat(enumValueString, slider.value);
    }

}
