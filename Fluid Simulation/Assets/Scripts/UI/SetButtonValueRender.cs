using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*SET BUTTON VALUE RENDER
 * 
 * Button calls for SPH Rendering
 */

public class SetButtonValueRender : MonoBehaviour
{
    public SPH.GPUVariables variable;
    [SerializeField] private SPHRendering smoothParticleHydroRendering;

    private void Awake()
    {
        SetValueFromButton(1);
    }

    public void SetValueFromButton(int val)
    {
        string enumValueString = Enum.GetName(typeof(SPH.GPUVariables), variable);
        smoothParticleHydroRendering.SetShaderFloat(enumValueString,val );
    }
}
