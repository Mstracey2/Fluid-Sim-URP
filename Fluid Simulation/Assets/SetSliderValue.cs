using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderValue : MonoBehaviour
{
    Slider slider;
    [SerializeField] private SPH smoothParticleHydro;
    public SPH.GPUVariables variable;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetValueFromSlider()
    {
        smoothParticleHydro.SetGpuFloat(variable, slider.value);
    }
}
