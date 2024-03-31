using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * DEMO SPACE PUTTY 
 * 
 * A script used to demonstrate the interesting capabilities of the project, 
 * applying a putty substance and setting the gravity to 0.
 */


public class DemoSpacePutty : ApplyPreset
{
    [SerializeField] private SPHParticleData puttyData; //putty (syrup)
    [SerializeField] private SPHParticleData defaultData;  //default fluid (water)
    [SerializeField] private Slider gravity;
    
    bool switcher = true;

    public void SwitchPutty()   //called from a UI button
    {
        if (switcher)
        {
            ApplySpacePutty(puttyData, 0);
        }
        else
        {
            ApplySpacePutty(defaultData, -10); // -10 is default gravity
        }
        switcher = !switcher;
    }

    /// <summary>
    /// Applies the data to the fluid, additional steps from inheritence.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="gravityVal"></param>
    public void ApplySpacePutty(SPHParticleData data, float gravityVal)
    {
        ApplyFluidPreset(data);
        gravity.value = gravityVal;
    }
}
