using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ParticleType", order = 1)]

/*
 * SPH PARTICLE DATA
 * 
 * A scriptable object script used to hold static particle data, 
 * user can select fluid presets using this. (water, syrup, oil)
 * This data is also useful for setting multiple fluids.
 */

public class SPHParticleData : ScriptableObject
{
    public float densityTarget;
    public float pressureForce;
    public float nearPressureForce;
    public float viscosity;
    public Color colour;
}
