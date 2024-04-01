

//This gradient type is used for the calculation of density. Theoretically, the peak is a spiked top with a harsh decline
float DensityKernal_SpikedGradient(float dist, float radius, float pi)
{
    if (dist < radius)
    {
        float scale = 15 / (2 * pi * pow(radius, 5));
        float vel = radius - dist;
        return vel * vel * scale;
    }
    return 0;
}

float NearDensityKernal_GreaterSpikedGradient(float dist, float radius, float pi)
{
    if (dist < radius)
    {
        float scale = 15 / (pi * pow(radius, 6));
        float vel = radius - dist;
        return vel * vel * vel * scale;
    }
    return 0;
}

/*
    These Derivative kernels are used to find the certain point within the density gradient. For example, if a particle is close to another, 
    the force will be stronger, meaning we need the point on the gradient that is somewhere around its peak.

    This kernel will calculate the exact point on that gradient, vital for the pressure value.
*/
float DerivativeDensityKernal_DerivativeSpikedGradient(float dist, float radius, float pi)
{
    if (dist <= radius)
    {
        float scale = 15 / (pow(radius, 5) * pi);
        float vel = radius - dist;
        return -vel * scale;
    }
    return 0;
}

float NearDerivativeDensityKernel_GreaterDerivativeSpikedGradient(float dist, float radius, float pi)
{
    if (dist <= radius)
    {
        float scale = 45 / (pow(radius, 6) * pi);
        float vel = radius - dist;
        return -vel * vel * scale;
    }
    return 0;
}

// similar to the density kernal but we square the values more to recieve a more smoothed and rounded peak on the gradient
float ViscosityKernal_RoundedGradient(float dist, float radius, float pi)
{
    if (dist < radius)
    {
        float scale = 315 / (64 * pi * pow(abs(radius), 9));
        float vel = radius * radius - dist * dist;
        return vel * vel * vel * scale;
    }
    return 0;
}
    
