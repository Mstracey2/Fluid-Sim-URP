using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*APPLY PRESET
 * 
 * Simple script that applies the UI slider values to particle presets
 * 
 * ITS BETTER TO APPLY THE DATA TO THE SLIDERS SO THEY ALSO CHANGE, 
 * UNITY DETECTS A CHANGE IN VALUE ANYWAY SO IT ALSO CALLS THE FUNCTION TO CHANGE THE PARTICLE VALUE
 */


public class ApplyPreset : MonoBehaviour
{
    //UI sliders for particle variables
    [SerializeField] private Slider densityTarget;
    [SerializeField] private Slider pressure;
    [SerializeField] private Slider nearPressure;
    [SerializeField] private Slider viscosity;

    public void ApplyFluidPreset(SPHParticleData data)
    {
        densityTarget.value = data.densityTarget;
        pressure.value = data.pressureForce;
        nearPressure.value = data.nearPressureForce;
        viscosity.value = data.viscosity;
    }
}
