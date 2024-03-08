using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddColourMaker : MonoBehaviour
{
    [SerializeField] private GameObject contentsParent;
    [SerializeField] private GameObject colourMaker;
    [SerializeField] private RenderingUI rendUI;
    public void AddGradientColour()
    {
        GameObject newColourMaker = Instantiate(colourMaker, contentsParent.transform);
        ColourMaker maker = newColourMaker.GetComponent<ColourMaker>();
        maker.rendUI = rendUI;
    }
}
