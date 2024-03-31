using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SPH;

/*SETSLIDERVALUE
 * 
 * Standard script attached to the UI sliders.
 * 
 * Passes the slider value to the SPH along with the variable to change in the compute shader.
 */

public class SetSliderValue : MonoBehaviour
{
    protected Slider slider;
    public SPH.GPUVariables variable;
    [SerializeField] private SPH sim;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        SetValueFromSlider();
    }

    public void SetValueFromSlider()
    {
        string enumValueString = Enum.GetName(typeof(SPH.GPUVariables), variable);      //gets the name of the enum
        sim.SetGpuFloat(enumValueString, slider.value);
    }

}
