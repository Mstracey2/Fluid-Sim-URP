using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddColourMaker : MonoBehaviour
{
    [SerializeField] private GameObject contentsParent;
    [SerializeField] private GameObject colourMaker;
    [SerializeField] private RenderingUI rendUI;
    private int gradientCount = 3;
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void AddGradientColour()
    {
        if (gradientCount < 8)
        {
            GameObject newColourMaker = Instantiate(colourMaker, contentsParent.transform);
            ColourMaker maker = newColourMaker.GetComponent<ColourMaker>();
            maker.rendUI = rendUI;
            gradientCount++;
        }
        else
        {
            button.interactable = false;
        }

    }
}
