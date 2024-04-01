

// calculates how much pressure is required in order for the particle to reach its target density.
float GetPressureFromDensity(float density, float index, float densTarget, float pressureMulti)
{
    return (density - densTarget) * pressureMulti;
}

float NearPressureFromDensity(float nearDensity, float index, float multiplier)
{
    return nearDensity * multiplier;
}

/*
    According to newtons third law:
    for every action (force) in nature there is an equal and opposite reaction.
    To do this, we must calculate a shared equal pressure by getting the mean avarage of both pressures.
*/
float GetSharedPressure(float denA, float denB, float index, float densTarget, float pressureMulti)
{
    float pressure1 = GetPressureFromDensity(denA, index, densTarget, pressureMulti);
    float pressure2 = GetPressureFromDensity(denB, index, densTarget, pressureMulti);
    return (pressure1 + pressure2) / 2;
}

float GetNearSharedPressure(float denA, float denB, float index, float multiplier)
{
    float pressure1 = NearPressureFromDensity(denA, index, multiplier);
    float pressure2 = NearPressureFromDensity(denB, index, multiplier);
    return (pressure1 + pressure2) / 2;
}

//Distance between two particles
float2 DistanceIn3DSpace(float3 particlePos, float3 neighborPos)
{
    float3 particleDiff = (particlePos - neighborPos);
    float particleOffset = dot(particleDiff, particleDiff);
    return (particleOffset, sqrt(particleOffset));
}