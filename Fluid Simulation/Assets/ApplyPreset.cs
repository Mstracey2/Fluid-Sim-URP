using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyPreset : MonoBehaviour
{
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
