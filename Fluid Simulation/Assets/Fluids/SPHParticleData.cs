using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ParticleType", order = 1)]
public class SPHParticleData : ScriptableObject
{
    public float densityTarget;
    public float pressureForce;
    public float nearPressureForce;
    public float viscosity;
}
