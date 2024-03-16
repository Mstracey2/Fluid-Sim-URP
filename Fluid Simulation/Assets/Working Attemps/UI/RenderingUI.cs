using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderingUI : MonoBehaviour
{
    [SerializeField] private SPHRendering simRend;
    public List<ColourMaker> uiGradientColours = new List<ColourMaker>();
    private GradientColorKey[] colourKeys;
    private GradientAlphaKey[] alphaKeys;
    Gradient colourGradient;
    public void UpdateGradient()
    {
        colourGradient = new Gradient();
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
        
        colourGradient.SetKeys(colourKeys, alphaKeys);
        simRend.ProduceColourGradientMap(colourGradient);
        
    }
}
