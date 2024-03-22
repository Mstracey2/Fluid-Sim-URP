using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoSpacePutty : ApplyPreset
{
    [SerializeField] private SPHParticleData puttyData;
    [SerializeField] private SPHParticleData waveData;
    [SerializeField] private Slider gravity;
    bool switcher = true;

    public void SwitchPutty()
    {
        
        if (switcher)
        {
            ApplySpacePutty(puttyData, 0);
        }
        else
        {
            ApplySpacePutty(waveData, -10);
        }
        switcher = !switcher;
    }

    public void ApplySpacePutty(SPHParticleData data, float gravityVal)
    {
        ApplyFluidPreset(data);
        gravity.value = gravityVal;
    }
}
