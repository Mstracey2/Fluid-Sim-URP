using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*COLOUR MAKER
 * 
 * Script that holds the primary functionality for the SPH Sim.
 * 
 * Calls the setup for the sim, 
 * along with dispatching compute shader kernels on a time step.
 */

public class ColourMaker : MonoBehaviour
{
    #region Variables
    public RenderingUI rendUI;
    public Color colour;
    [SerializeField] private Image img;

    private float rCol;
    private float gCol;
    private float bCol;

    public bool alreadyInstanced;   // ignores adding it to the rend list if its already in the list
    #endregion

    private void Start()
    {
        rCol = img.color.r;
        gCol = img.color.g;
        bCol = img.color.b;

        // .Find is not ideal here but only occurs when the colour maker is first instanciated
        rendUI = GameObject.Find("UISettings").GetComponent<RenderingUI>();
        if (!alreadyInstanced)
        {
            rendUI.uiGradientColours.Add(this);
        }
        UpdateColour();     
    }

    #region RGB Change
    public void UpdateRed(Slider slider)
    {
        rCol = slider.value;
        UpdateColour();
    }

    public void UpdateBlue(Slider slider)
    {
        bCol = slider.value;
        UpdateColour();
    }

    public void UpdateGreen(Slider slider)
    {
        gCol = slider.value;
        UpdateColour();
    }
    #endregion

    public void UpdateColour()
    {
        colour = new Color(rCol, gCol, bCol, 1);
        img.color = colour;
        rendUI.UpdateGradient();        //updates the gradient with the new colour added
    }
}
