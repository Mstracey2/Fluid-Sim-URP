using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourMaker : MonoBehaviour
{
    public RenderingUI rendUI;
    public Color colour;
    [SerializeField] private Image img;

    private float rCol;
    private float gCol;
    private float bCol;

    private void Start()
    {
        rCol = img.color.r;
        gCol = img.color.g;
        bCol = img.color.b;
        rendUI = GameObject.Find("UISettings").GetComponent<RenderingUI>();
        rendUI.uiGradientColours.Add(this);
        UpdateColour();
    }

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
    public void UpdateColour()
    {
        colour = new Color(rCol, gCol, bCol, 1);
        img.color = colour;
        rendUI.UpdateGradient();
    }
}
