using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddColourMaker : MonoBehaviour
{
    [SerializeField] private GameObject contentsParent;
    [SerializeField] private GameObject colourMaker;
    [SerializeField] private RenderingUI rendUI;
    [SerializeField] private Button removeButton;
    Button addButton;


    private int gradientCount = 3;

    private void Awake()
    {
        addButton = GetComponent<Button>();
    }
    public void AddGradientColour()
    {
        if (gradientCount < 8)
        {
            if (removeButton.interactable == false) { removeButton.interactable = true; }

            GameObject newColourMaker = Instantiate(colourMaker, contentsParent.transform);
            ColourMaker maker = newColourMaker.GetComponent<ColourMaker>();
            newColourMaker.transform.SetSiblingIndex(gradientCount+2);
            maker.rendUI = rendUI;
            gradientCount++;

            if (gradientCount == 8) { addButton.interactable = false; }
        }
    }

    public void RemoveGradientColour()
    {
        if(gradientCount > 2)
        {
            if (addButton.interactable == false) { addButton.interactable = true; }

            Destroy(rendUI.uiGradientColours[gradientCount - 1].gameObject);
            rendUI.uiGradientColours.RemoveAt(gradientCount - 1);
            gradientCount--;

            if (gradientCount == 2) { removeButton.interactable = false; }
        }
    }
}
