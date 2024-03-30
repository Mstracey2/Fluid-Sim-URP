using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderValueRender : MonoBehaviour
{
    protected Slider slider;
    public SPH.GPUVariables variable;
    [SerializeField] private SPHRendering smoothParticleHydroRend;


    private void Awake()
    {
        slider = GetComponent<Slider>();
        SetValueFromSlider();
    }

    public void SetValueFromSlider()
    {
        string enumValueString = Enum.GetName(typeof(SPH.GPUVariables), variable);
        smoothParticleHydroRend.SetShaderFloat(enumValueString, slider.value);
    }
}

