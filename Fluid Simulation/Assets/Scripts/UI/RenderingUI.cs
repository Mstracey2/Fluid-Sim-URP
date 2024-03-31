using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*RENDERING UI
 * 
 *  Converts the colours from the colour makers to a gradient
 */

public class RenderingUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private SPHRendering simRend;
    public List<ColourMaker> uiGradientColours = new List<ColourMaker>();
    private GradientColorKey[] colourKeys;
    private GradientAlphaKey[] alphaKeys;
    Gradient colourGradient;
    #endregion

    public void UpdateGradient()
    {
        colourGradient = new Gradient();
        colourKeys = new GradientColorKey[uiGradientColours.Count];
        alphaKeys = new GradientAlphaKey[uiGradientColours.Count];

        int count = uiGradientColours.Count - 1;    //-1 is to set the keys in a suitable pos (2 keys make the keys sit at the start and end of the gradient)

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
                //math to set the keys in the correct pos on the gradient depending on the amount of colours
                colourKeys[i].time = (1f / count) * i;
                alphaKeys[i].time = (1f / count) * i;
            }

        }
        
        colourGradient.SetKeys(colourKeys, alphaKeys);
        simRend.ProduceColourGradientMap(colourGradient);
        
    }
}
