using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*SET SLIDER VALUE RENDER
 * 
 * Setting slider values for SPH rendering
 * 
 */

public class SetSliderValueRender : MonoBehaviour
{
    #region Variables
    protected Slider slider;
    public SPH.GPUVariables variable;
    [SerializeField] private SPHRendering smoothParticleHydroRend;
    #endregion

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

