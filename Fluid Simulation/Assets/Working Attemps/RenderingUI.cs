using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderingUI : MonoBehaviour
{
    [SerializeField] private SPH sim;
    public List<ColourMaker> uiGradientColours = new List<ColourMaker>();
    private GradientColorKey[] colourKeys;
    private GradientAlphaKey[] alphaKeys;
    public void UpdateGradient()
    {
        colourKeys = new GradientColorKey[uiGradientColours.Count];
        alphaKeys = new GradientAlphaKey[uiGradientColours.Count];
        int count = uiGradientColours.Count - 1;
        for (int i = 0; i < uiGradientColours.Count; i++)
        {
            colourKeys[i].color = uiGradientColours[i].colour;
            
            alphaKeys[i].alpha = 1;
            if (i == 0)
            {
                colourKeys[i].time = 0f;
            }
            else
            {
                colourKeys[i].time = (1f / count) * i;
                alphaKeys[i].time = (1f / count) * i;
            }

        }
        sim.colourGradient.SetKeys(colourKeys, alphaKeys);
        
    }

    public void UpdateRenderSize(Slider sl)
    {
        sim.particleRenderSize = sl.value;
    }

    public void UpdateResolution(Slider sl)
    {
        sim.resolution = (int)sl.value;
    }
    public void UpdateVelocityMax(Slider sl)
    {
        sim.particleMaxVelocity = (int)sl.value;
    }
}
